using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;

namespace OrderElimination
{
    public class ScenesMediator : SerializedMonoBehaviour
    {
        [ShowInInspector, OdinSerialize]
        private IGameCharacterTemplate[] _testPlayerCharacters;

        [ShowInInspector, OdinSerialize]
        private IGameCharacterTemplate[] _testEnemyCharacters;

        [OdinSerialize]
        private BattleScenario _testScenario;
        
        private readonly Dictionary<string, object> _data = new();
        
        public T Get<T>(string name)
        {
            if (!_data.ContainsKey(name.ToLower()))
                throw new ArgumentException("Theres no registered object with name " + name);

            var type = typeof(T);
            if(_data.Values.All(x => !type.IsInstanceOfType(x)))
                throw new ArgumentException("Theres no registered object with type " + typeof(T).Name);
            
            return (T)_data[name];
        }

        public bool Contains<T>(string name)
        {
            if (!_data.ContainsKey(name.ToLower()))
                return false;

            var type = typeof(T);
            return _data.Values.Any(x => type.IsInstanceOfType(x));
        }
        
        public void Unregister(string name) => _data.Remove(name.ToLower());
        
        public void Register(string name, object obj) => _data[name.ToLower()] = obj;

        public void InitTest()
        {
            Register("player characters", GameCharactersFactory.CreateGameEntities(_testPlayerCharacters));
            Register("enemy characters", GameCharactersFactory.CreateGameEntities(_testEnemyCharacters));
            Register("scenario", _testScenario);
            Register("stats", new StrategyStats());
        }

        private void Awake()
        {
            if(FindObjectsOfType<ScenesMediator>().Length > 1)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);

            InitTest();
        }
    }
}