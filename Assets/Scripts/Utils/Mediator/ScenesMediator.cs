using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.Battle;
using OrderElimination.GameContent;
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
        private BattleFieldLayout _testScenario;

        [OdinSerialize]
        private IBattleRules _testRules;

        [SerializeField]
        private bool _test;
        
        private readonly Dictionary<string, object> _data = new();

        private static ScenesMediator s_instance;
        
        public T Get<T>(MediatorRegistration registration)
        {
            var name = registration.ToString().ToLower();
            if (!_data.ContainsKey(name))
                throw new ArgumentException($"Theres no registered object with name \"{name}\"");

            var type = typeof(T);
            if(_data.Values.All(x => !type.IsInstanceOfType(x)))
                throw new ArgumentException($"Theres no registered object of type \"{typeof(T).Name}\"");
            
            return (T)_data[name];
        }

        public bool Contains<T>(MediatorRegistration registration)
        {
            if (!_data.ContainsKey(registration.ToString().ToLower()))
                return false;

            var type = typeof(T);
            return _data.Values.Any(x => type.IsInstanceOfType(x));
        }

        public void Unregister(MediatorRegistration registration) 
            => _data.Remove(registration.ToString().ToLower());
        
        public void Register<T>(MediatorRegistration registration, T obj) 
            => _data[registration.ToString().ToLower()] = obj;

        #region StringInteraction
        private bool Contains<T>(string name)
        {
            if (!_data.ContainsKey(name.ToLower()))
                return false;

            var type = typeof(T);
            return _data.Values.Any(x => type.IsInstanceOfType(x));
        }
        private T Get<T>(string name)
        {
            if (!_data.ContainsKey(name.ToLower()))
                throw new ArgumentException("Theres no registered object with name " + name);

            var type = typeof(T);
            if (_data.Values.All(x => !type.IsInstanceOfType(x)))
                throw new ArgumentException("Theres no registered object with type " + typeof(T).Name);

            return (T)_data[name.ToLower()];
        }
        private void Register<T>(string name, T obj) => _data[name.ToLower()] = obj;
        private void Unregister(string name) => _data.Remove(name.ToLower());
        #endregion

        public void InitTest()
        {
            if (!Contains<GameCharacter[]>(MediatorRegistration.PlayerCharacters)
                && _testPlayerCharacters != null)
                Register(MediatorRegistration.PlayerCharacters, GameCharactersFactory.CreateGameCharacters(_testPlayerCharacters));
            if (!Contains<GameCharacter[]>(MediatorRegistration.EnemyCharacters) 
                && _testEnemyCharacters != null)
                Register(MediatorRegistration.EnemyCharacters, GameCharactersFactory.CreateGameCharacters(_testEnemyCharacters));
            if (!Contains<IBattleFieldLayout>(MediatorRegistration.BattleField) 
                && _testScenario != null)
                Register(MediatorRegistration.BattleField, _testScenario);
            if (!Contains<IBattleRules>(MediatorRegistration.BattleRules)
                && _testRules != null)
                Register(MediatorRegistration.BattleRules, _testRules);
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
        }
    }
}