using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace OrderElimination.SavesManagement
{
    public class BinaryFileSerializer : IFileSerializer
    {
        private IFormatter _formatter = new BinaryFormatter();

        public T Deserialize<T>(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open);
            var deserializedObject = _formatter.Deserialize(fileStream);
            fileStream.Close();
            return (T)deserializedObject;
        }

        public void Serialize<T>(string path, T obj)
        {
            var fileStream = new FileStream(path, FileMode.CreateNew);
            _formatter.Serialize(fileStream, obj);
            fileStream.Close();
        }
    }
}
