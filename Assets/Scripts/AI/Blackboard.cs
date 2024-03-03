using System.Collections.Generic;
using OrderElimination;

namespace AI
{
    public class Blackboard
    {
        private readonly Blackboard _parent;
        private readonly Dictionary<string, object> _data = new();

        public Blackboard(Blackboard parent = null)
        {
            _parent = parent;
        }
        
        public T Get<T>(string name)
        {
            if (_data.TryGetValue(name, out var value) && (T)value != null)
                return (T)value;
            if (_parent != null)
                return _parent.Get<T>(name);
            
            Logging.LogError($"Variable with name {name} was not found");
            return default;
        }

        public bool TrySetVariable(string name, object obj)
        {
            if (!_data.ContainsKey(name))
            {
                Logging.LogError($"Variable with name {name} was not found");
                return false;
            }
            
            _data[name] = obj;
            return true;

        }

        public bool Register(string name, object obj)
        {
            if (_data.TryAdd(name, obj)) 
                return true;
            
            Logging.LogError($"Variable with name {name} already registered");
            return false;
        }
    }
}