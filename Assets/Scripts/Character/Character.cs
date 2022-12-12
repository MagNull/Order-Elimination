using CharacterAbility;
using UnityEngine;
using UnityEngine.Serialization;

namespace OrderElimination
{
    [CreateAssetMenu(fileName = "CharacterInfo", menuName = "Character")]
    public class Character : ScriptableObject, IBattleCharacterInfo
    {
        [SerializeField]
        private BattleStats _battleStats;
        // [SerializeField]
        private StrategyStats _strategyStats;
        [SerializeField]
        private Sprite _view;
        [FormerlySerializedAs("_abilities")]
        [SerializeField]
        private AbilityInfo[] _activeAbilities;
        [SerializeField]
        private AbilityInfo[] _passiveAbilities;
        
        public BattleStats GetBattleStats() => _battleStats;

        public Sprite GetView() => _view;

        public AbilityInfo[] GetActiveAbilityInfos() => _activeAbilities;
        public AbilityInfo[] GetPassiveAbilityInfos() => _passiveAbilities;

        public StrategyStats GetStrategyStats() => _strategyStats;
        public void RaiseExperience(float experience)
        {
            throw new System.NotImplementedException();
        }
    }
}