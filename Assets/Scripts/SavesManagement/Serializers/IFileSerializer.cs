namespace OrderElimination.SavesManagement
{
    public interface IFileSerializer
    {
        public void Serialize(string path, object obj);

        public object Deserialize(string path);
    }
}
