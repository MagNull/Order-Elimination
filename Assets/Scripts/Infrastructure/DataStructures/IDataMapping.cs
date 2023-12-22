namespace OrderElimination.Infrastructure
{
    public interface IDataMapping<TKey, TData>
    {
        public bool ContainsKey(TKey key);

        public bool ContainsData(TData data);

        public TData GetData(TKey key);

        public TKey GetKey(TData data);
    }
}
