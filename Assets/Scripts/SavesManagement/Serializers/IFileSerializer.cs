namespace OrderElimination.SavesManagement
{
    public interface IFileSerializer
    {
        public void Serialize<T>(string path, T obj);

        public T Deserialize<T>(string path);
    }
}
