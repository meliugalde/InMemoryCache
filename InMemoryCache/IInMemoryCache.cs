namespace InMemoryCache
{
    public interface IInMemoryCache<K,T>
    {
        void Add(K key, T value);
        T Get(K key);
    }
}