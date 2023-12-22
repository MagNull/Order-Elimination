using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public class JsonFileSerializer : IFileSerializer
    {
        public object Deserialize(string path)
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject(json);
        }

        public void Serialize(string path, object obj)
        {
            var json = JsonConvert.SerializeObject(obj);//JsonUtility.ToJson(obj);
            File.WriteAllText(path, json);
        }
    }
}
