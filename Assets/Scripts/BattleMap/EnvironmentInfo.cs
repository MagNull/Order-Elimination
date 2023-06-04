using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination.BM
{
    [CreateAssetMenu( fileName = "Environment Object", menuName = "Battle/Environment Object" )]
    public class EnvironmentInfo : SerializedScriptableObject, IBattleStructureData
    {
        [SerializeField]
        private string _name;
        [PreviewField(100, ObjectFieldAlignment.Left)]
        [SerializeField]
        private Sprite _spriteView;
        [SerializeField]
        private float _maxHealth;
        [SerializeField]
        private PassiveAbilityBuilder[] _posessedAbilities;
        [SerializeField]
        private IBattleObstacleSetup _obstacleSetup;

        public string Name => _name;
        public Sprite BattleIcon => _spriteView;
        public float MaxHealth => _maxHealth;
        public IBattleObstacleSetup ObstacleSetup => _obstacleSetup;
        public PassiveAbilityBuilder[] GetPossesedAbilities() => _posessedAbilities;
    }
}