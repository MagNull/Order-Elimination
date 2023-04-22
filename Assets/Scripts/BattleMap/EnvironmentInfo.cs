using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination.BM
{
    [CreateAssetMenu( fileName = "Environment Object", menuName = "Map/Environment Object" )]
    public class EnvironmentInfo : SerializedScriptableObject
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private Sprite _spriteView;
        [SerializeField]
        private float _maxHealth;
        [SerializeField]
        public AbilityBuilderData[] _posessedAbilities;
        [SerializeField]
        private bool _isWalkable;
        [SerializeField]
        private BattleStats _battleStats;
        [SerializeField]
        private ITickEffect[] _enterEffects;

        public string Name => _name;
        public Sprite BattleIcon => _spriteView;
        public float MaxHealth => _maxHealth;
        public AbilityBuilderData[] GetActiveAbilities() => _posessedAbilities;

        public BattleStats Stats => _battleStats;
        public ITickEffect[] EnterEffects => _enterEffects;

        public bool IsWalkable => _isWalkable;
    }
}