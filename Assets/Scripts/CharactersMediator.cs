using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination
{
    public class CharactersMediator : SerializedMonoBehaviour
    {
        [OdinSerialize]
        private List<IBattleCharacterInfo> _battleStatsList;

        [OdinSerialize] private List<IBattleCharacterInfo> _enemies;

        private void Awake() => DontDestroyOnLoad(gameObject);

        public void SetSquad(List<IBattleCharacterInfo> battleStatsList) => _battleStatsList = battleStatsList;
        public void SetEnemies(List<IBattleCharacterInfo> battleStatsList) => _enemies = battleStatsList;

        public List<IBattleCharacterInfo> GetBattleCharactersInfo() => _battleStatsList;
        public List<IBattleCharacterInfo> GetBattleEnemyInfo() => _enemies;
    }
}