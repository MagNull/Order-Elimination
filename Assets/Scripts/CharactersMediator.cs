using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination
{
    public class CharactersMediator : SerializedMonoBehaviour
    {
        [ShowInInspector, OdinSerialize]
        private List<IGameCharacterTemplate> _testPlayerCharacters = new();

        [ShowInInspector, OdinSerialize]
        private List<IGameCharacterTemplate> _testEnemyCharacters = new();

        private List<GameCharacter> _playerCharacters;
        private List<GameCharacter> _enemyCharacters;

        [OdinSerialize]
        public BattleScenario BattleScenario { get; private set; }

        public IEnumerable<GameCharacter> GetPlayerCharacters()
            => _playerCharacters ?? GameCharactersFactory.CreateGameEntities(_testPlayerCharacters);
        public IEnumerable<GameCharacter> GetEnemyCharacters()
            => _enemyCharacters ?? GameCharactersFactory.CreateGameEntities(_testEnemyCharacters);
        public void SetPlayerCharacters(IEnumerable<GameCharacter> battleStatsList)
            => _playerCharacters = battleStatsList.ToList();
        public void SetEnemyCharacters(IEnumerable<GameCharacter> battleStatsList)
            => _enemyCharacters = battleStatsList.ToList();
        public void SetScenario(BattleScenario scenario) => BattleScenario = scenario;

        private void Awake()
        {
            if(FindObjectsOfType<CharactersMediator>().Length > 1)
                Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }
    }
}