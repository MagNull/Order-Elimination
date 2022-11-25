using CharacterAbility;
using CharacterAbility.BuffEffects;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination.BattleMap
{
    [CreateAssetMenu( fileName = "Environment Object", menuName = "Map/Environment Object" )]
    public class EnvironmentInfo : SerializedScriptableObject
    {
        [SerializeField]
        private Sprite _spriteView;
        [SerializeField]
        private bool _isWalkable;
        [SerializeField]
        private BattleStats _battleStats;
        [SerializeField]
        private ITickEffect[] _enterEffects;

        public BattleStats Stats => _battleStats;

        public Sprite SpriteView => _spriteView;

        public ITickEffect[] EnterEffects => _enterEffects;

        public bool IsWalkable => _isWalkable;
    }
}