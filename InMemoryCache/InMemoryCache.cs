using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InMemoryCache
{
    public sealed class InMemoryCache<K,T> : IInMemoryCache<K,T>
    {
        private int Capacity;
        private IDictionary<K,LinkedListNode<CacheItem<K,T>>> Cache;
        private LinkedList<CacheItem<K,T>> LRU;
        private readonly SemaphoreSlim Semaphore;
        private static readonly object _lock = new object();
        private static bool _isInitialized = false;

        private const int DEFAULT_CAPACITY = 100;

        private static Lazy<InMemoryCache<K, T>> instance = 
            new Lazy<InMemoryCache<K, T>>(() => new InMemoryCache<K, T>());
        public static InMemoryCache<K, T> Instance => instance.Value;

        private InMemoryCache() 
        { 
            Capacity = DEFAULT_CAPACITY;
            Cache = new ConcurrentDictionary<K, LinkedListNode<CacheItem<K, T>>>();
            LRU = new LinkedList<CacheItem<K, T>>();
            Semaphore = new SemaphoreSlim(1, 1);
        }

        public static void Initialize(int capacity)
        {
            lock(_lock)
            {
                if (capacity < 1)
                    throw new InvalidOperationException("Capacity should be greater than 0");
                if (_isInitialized)
                    throw new InvalidOperationException("Cache has already been initialized.");

                instance.Value.Capacity = capacity;
                _isInitialized = true;
            }           
        }

        public void Add(K key, T value)
        {
            Semaphore.Wait();
            try
            {
                if (Cache.TryGetValue(key, out LinkedListNode<CacheItem<K,T>> cacheItem))
                {
                    UpdateRecentlyUsed(cacheItem);
                    cacheItem.Value.Value = value;
                }
                else
                {
                    if (Cache.Count == Capacity)
                    {
                        EvictLeastRecentlyUsed();
                    }

                    LinkedListNode<CacheItem<K, T>> newCacheItem = 
                        new LinkedListNode<CacheItem<K, T>>(new CacheItem<K,T>(key, value));
                    Cache.Add(key, newCacheItem);
                    LRU.AddFirst(newCacheItem);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private void EvictLeastRecentlyUsed()
        {
            CacheItem<K,T> last = LRU.Last();
            LRU.RemoveLast();
            Cache.Remove(last.Key);
        }

        public T Get(K key)
        {
            Semaphore.Wait();
            
            try
            {
                if(Cache.TryGetValue(key, out LinkedListNode<CacheItem<K, T>> cacheItem))
                {
                    UpdateRecentlyUsed(cacheItem);
                    return cacheItem.Value.Value;
                }
            }
            finally 
            { 
                Semaphore.Release(); 
            }

            return default;
        }

        private void UpdateRecentlyUsed(LinkedListNode<CacheItem<K, T>> cacheItemNode)
        {
            LRU.Remove(cacheItemNode);
            LRU.AddFirst(cacheItemNode);
        }

        public void Dispose()
        {
            lock (_lock)
            {
                instance.Value.Cache.Clear();
                instance.Value.LRU.Clear();
                _isInitialized = false;
            }

        }
    }

    
}
