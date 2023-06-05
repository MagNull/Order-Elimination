using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;
using RoguelikeMap.Points;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination
{
    public class CharactersMediator : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private List<IBattleCharacterInfo> _playerCharacters;//Change to GameCharacter
        [OdinSerialize] 
        private List<IBattleCharacterInfo> _enemies;
        [OdinSerialize]
        public BattleScenario BattleScenario { get; private set; }

        public IEnumerable<GameCharacter> GetPlayerCharactersInfo() => GameCharactersFactory.CreateGameEntities(_playerCharacters);
        public IEnumerable<GameCharacter> GetEnemyCharactersInfo() => GameCharactersFactory.CreateGameEntities(_enemies);
        public void SetSquad(IEnumerable<IBattleCharacterInfo> battleStatsList)
            => _playerCharacters = battleStatsList.ToList();
        public void SetEnemies(IEnumerable<IBattleCharacterInfo> battleStatsList)
            => _enemies = battleStatsList.ToList();
        public void SetScenario(BattleScenario scenario) => BattleScenario = scenario;


        private void Awake() => DontDestroyOnLoad(gameObject);

        private int _pointNumber;
        public int PointNumber => _pointNumber;
        public void SetPointNumber(int pointNumber) => _pointNumber = pointNumber;
    }
}