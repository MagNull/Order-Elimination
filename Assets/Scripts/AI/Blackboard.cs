using System.Collections.Generic;

namespace AI
{
    public class Blackboard
    {
        private Blackboard _parent;
        private readonly Dictionary<string, object> _data = new();

        public Blackboard(Blackboard parent = null)
        {
            _parent = parent;
        }
        
        public T Get<T>(string name)
        {
            if (_data.TryGetValue(name, out var value))
                return (T)value;
            if (_parent != null)
                return _parent.Get<T>(name);
            
            return default;
        }

        public object Register(string name, object obj) => _data[name] = obj;
    }
}