using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.MetaGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination
{
    public class CharactersMediator : SerializedMonoBehaviour
    {
        [ShowInInspector, OdinSerialize]
        private List<IGameCharacterData> _testPlayerCharacters = new();

        [ShowInInspector, OdinSerialize]
        private List<IGameCharacterData> _testEnemyCharacters = new();

        //[HideInInspector, OdinSerialize]
        private List<GameCharacter> _playerCharacters;
        //[HideInInspector, OdinSerialize] 
        private List<GameCharacter> _enemyCharacters;
        [OdinSerialize]
        public BattleScenario BattleScenario { get; private set; }

        public IEnumerable<GameCharacter> GetPlayerCharactersInfo()
            => _playerCharacters ?? GameCharactersFactory.CreateGameEntities(_testPlayerCharacters);

        public IEnumerable<GameCharacter> GetEnemyCharactersInfo()
            => _enemyCharacters ?? GameCharactersFactory.CreateGameEntities(_testEnemyCharacters);
        public void SetPlayerSquad(IEnumerable<GameCharacter> battleStatsList)
            => _playerCharacters = battleStatsList.ToList();
        public void SetEnemies(IEnumerable<GameCharacter> battleStatsList)
            => _enemyCharacters = battleStatsList.ToList();
        public void SetScenario(BattleScenario scenario) => BattleScenario = scenario;


        private void Awake() => DontDestroyOnLoad(gameObject);
    }
}