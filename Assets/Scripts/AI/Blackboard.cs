using System.Collections.Generic;

namespace AI
{
    public class Blackboard
    {
        private readonly Dictionary<string, object> _data = new();

        public T Get<T>(string name)
        {
            return _data.ContainsKey(name) && ((T)_data[name]) != null ? (T)_data[name] : default;
        }

        public object Register(string name, object obj) => _data[name] = obj;
    }
}