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
        private string _name;
        [SerializeField]
        private Sprite _viewIcon;
        [SerializeField]
        private Sprite _viewAvatar;
        [FormerlySerializedAs("_abilities")]
        [SerializeField]
        private AbilityInfo[] _activeAbilities;
        [SerializeField]
        private AbilityInfo[] _passiveAbilities;
        
        public BattleStats GetBattleStats() => _battleStats;

        public string GetName() => _name;
        public Sprite GetViewIcon() => _viewIcon;
        public Sprite GetViewAvatar() => _viewAvatar;

        public AbilityInfo[] GetActiveAbilityInfos() => _activeAbilities;
        public AbilityInfo[] GetPassiveAbilityInfos() => _passiveAbilities;

        public StrategyStats GetStrategyStats() => _strategyStats;
        public void RaiseExperience(float experience)
        {
            throw new System.NotImplementedException();
        }
    }
}