using CharacterAbility;
using CharacterAbility.BuffEffects;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination.BM
{
    [PropertyTooltip("@$value." + nameof(Name))]
    [CreateAssetMenu( fileName = "Environment Object", menuName = "Battle/Environment Object" )]
    public class EnvironmentInfo : SerializedScriptableObject, IBattleStructureTemplate
    {
        [SerializeField]
        private string _name;
        [PreviewField(100, ObjectFieldAlignment.Left)]
        [SerializeField]
        private Sprite _spriteView;
        [PreviewField(100, ObjectFieldAlignment.Left)]
        [AssetsOnly]
        [SerializeField]
        private GameObject _visualModel;
        [SerializeField]
        private float _maxHealth;
        [SerializeField]
        private PassiveAbilityBuilder[] _posessedAbilities;
        [SerializeField]
        private IBattleObstacleSetup _obstacleSetup;

        public string Name => _name;
        public Sprite BattleIcon => _spriteView;
        public GameObject VisualModel => _visualModel;
        public float MaxHealth => _maxHealth;
        public IBattleObstacleSetup ObstacleSetup => _obstacleSetup;
        public PassiveAbilityBuilder[] GetPossesedAbilities() => _posessedAbilities;
    }
}