namespace OrderElimination.SavesManagement
{
    public interface IDataMapping<TKey, TData>
    {
        public TData GetData(TKey key);

        public TKey GetKey(TData data);
    }
}
