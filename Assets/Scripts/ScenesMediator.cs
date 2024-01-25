using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.Battle;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination
{
    public class ScenesMediator : SerializedMonoBehaviour
    {
        [ShowInInspector, OdinSerialize]
        private IGameCharacterTemplate[] _testPlayerCharacters;

        [ShowInInspector, OdinSerialize]
        private IGameCharacterTemplate[] _testEnemyCharacters;

        [OdinSerialize]
        private IBattleMapLayout _testScenario;

        [OdinSerialize]
        private IBattleRules _testRules;

        [SerializeField]
        private bool _test;
        
        private readonly Dictionary<string, object> _data = new();

        private static ScenesMediator s_instance;
        
        public T Get<T>(string name)
        {
            if (!_data.ContainsKey(name.ToLower()))
                throw new ArgumentException("Theres no registered object with name " + name);

            var type = typeof(T);
            if(_data.Values.All(x => !type.IsInstanceOfType(x)))
                throw new ArgumentException("Theres no registered object with type " + typeof(T).Name);
            
            return (T)_data[name.ToLower()];
        }

        public bool Contains<T>(string name)
        {
            if (!_data.ContainsKey(name.ToLower()))
                return false;

            var type = typeof(T);
            return _data.Values.Any(x => type.IsInstanceOfType(x));
        }
        
        public void Unregister(string name) => _data.Remove(name.ToLower());
        
        public void Register<T>(string name, T obj) => _data[name.ToLower()] = obj;

        public void InitTest()
        {
            var playerChars = "player characters";
            var enemyChars = "enemy characters";
            var scenario = "scenario";
            var rules = "rules";
            //if (_testEnemyCharacters == null || _testEnemyCharacters.Any(c => c == null))
            //    Logging.LogWarning("Test enemy characters null");
            if (!Contains<GameCharacter[]>(playerChars))
                Register(playerChars, GameCharactersFactory.CreateGameCharacters(_testPlayerCharacters));
            if (!Contains<GameCharacter[]>(enemyChars) && _testEnemyCharacters != null)
                Register(enemyChars, GameCharactersFactory.CreateGameCharacters(_testEnemyCharacters));
            if (!Contains<IBattleMapLayout>(scenario))
                Register(scenario, _testScenario);
            if (!Contains<IBattleRules>(rules))
                Register(rules, _testRules);
        }

        private void Awake()
        {
            if (s_instance && s_instance != this)
                Destroy(gameObject);
            else
                s_instance = this;
            DontDestroyOnLoad(gameObject);

            if (_test)
                InitTest();

            var rules = "rules";
            if (!Contains<IBattleRules>(rules))
                Register(rules, _testRules);
        }
    }
}