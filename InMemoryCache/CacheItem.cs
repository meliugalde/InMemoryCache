namespace InMemoryCache
{
    public class CacheItem<K, T>
    {
        public K Key { get; }
        public T Value { get; set; }

        public CacheItem(K key, T value)
        {
            Key = key;
            Value = value;
        }
    }
}
