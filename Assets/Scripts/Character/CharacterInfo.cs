using CharacterAbility;
using UnityEngine;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character")]
    public class CharacterInfo : ScriptableObject, IBattleCharacterInfo, ISquadMember
    {
        [SerializeField]
        private BattleStats _battleStats;
        // [SerializeField]
        private StrategyStats _strategyStats;
        [SerializeField]
        private Sprite _view;
        [SerializeField]
        private AbilityInfo[] _abilities;
        
        public BattleStats GetBattleStats() => _battleStats;

        public Sprite GetView() => _view;

        public AbilityInfo[] GetAbilityInfos() => _abilities;
        public StrategyStats GetStrategyStats() => _strategyStats;
        public void RaiseExperience(float experience)
        {
            throw new System.NotImplementedException();
        }
    }
}