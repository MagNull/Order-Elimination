using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public class BinaryFileSerializer : IFileSerializer
    {
        private IFormatter _formatter = new BinaryFormatter();

        public object Deserialize(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open);
            var deserializedObject = _formatter.Deserialize(fileStream);
            fileStream.Close();
            return deserializedObject;
        }

        public void Serialize(string path, object obj)
        {
            var fileStream = new FileStream(path, FileMode.CreateNew);
            _formatter.Serialize(fileStream, obj);
            fileStream.Close();
        }
    }
}
