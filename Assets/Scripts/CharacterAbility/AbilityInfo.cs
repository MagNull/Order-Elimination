using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace CharacterAbility
{
    [Serializable]
    public enum AbilityEffectType
    {
        Damage,
        Heal,
        Move,
        OverTimeEffect,
        Modificator
    }

    public enum OverTimeAbilityType
    {
        Damage,
        Heal,
    }

    public enum ModificationType
    {
        Accuracy
    }

    public enum TargetType
    {
        Self,
        Enemy,
        Ally,
        All,
        Empty
    }

    [Serializable]
    public struct AbilityEffect
    {
        public AbilityEffectType Type;
        [ShowIf("@Type == AbilityEffectType.Damage || Type == AbilityEffectType.Heal")]
        public int Amounts;
        [ShowIf("@Type == AbilityEffectType.Damage || Type == AbilityEffectType.Heal")]
        public float AttackScale;
        [ShowIf("@Type == AbilityEffectType.Modificator")]
        public ModificationType Modification;
        [ShowIf("@Type == AbilityEffectType.Modificator")]
        public int ModificationValue;
        [ShowIf("@Type == AbilityEffectType.OverTimeEffect")]
        public OverTimeAbilityType OverTimeType;
        [ShowIf("@Type == AbilityEffectType.OverTimeEffect")]
        public int Duration;
        [ShowIf("@Type == AbilityEffectType.OverTimeEffect")]
        public int TickValue;
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Ability Info")]
    public class AbilityInfo : SerializedScriptableObject
    {
        [Title("General Parameters")]
        [SerializeField]
        [Range(0, 10)]
        private int _coolDown;
        [SerializeField]
        [Range(0, 10)]
        private int _startCoolDown;

        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: SerializeField] public ActionType ActionType { get; private set; }

        #region Params

        [HideInInspector]
        [SerializeField]
        private bool _hasTarget;
        [HideInInspector]
        [SerializeField]
        private bool _hasTargetEffect;

        [Title("Type Specific Parameters")]
        [ShowIf("_hasTarget")]
        [SerializeField]
        private bool _distanceFromMovement;
        [ShowIf("@_hasTarget && !_distanceFromMovement")]
        [SerializeField]
        private int _distance;
        [ShowIf("_hasTarget")]
        [SerializeField]
        private TargetType _targetType;

        [ShowIf("_hasTargetEffect")]
        [SerializeField]
        private AbilityEffect[] _targetEffects;

        [HideInInspector]
        [SerializeField]
        private bool _hasAreaEffect;

        [ShowIf("_hasAreaEffect")]
        [SerializeField]
        private int _areaRadius;

        [ShowIf("_hasAreaEffect")]
        [SerializeField]
        private AbilityEffect[] _areaEffects;

        #endregion

        #region Properties

        public TargetType TargetType => _targetType;
        public int Distance => _distance;

        public AbilityEffect[] TargetEffects => _targetEffects;

        public int AreaRadius => _areaRadius;

        public AbilityEffect[] AreaEffects => _areaEffects;

        public bool HasAreaEffect => _hasAreaEffect;

        public bool HasTargetEffect => _hasTargetEffect;

        public bool DistanceFromMovement => _distanceFromMovement;

        public int CoolDown => _coolDown + 1;

        public int StartCoolDown => _startCoolDown + 1;

        #endregion

        [HideIf("_hasTarget"), Button]
        private void AddTarget() => _hasTarget = true;

        [ShowIf("_hasTarget"), Button]
        private void RemoveTarget()
        {
            _hasTarget = false;
            RemoveTargetEffect();
        }

        [HideIf("_hasAreaEffect"), Button]
        private void AddAreaEffect() => _hasAreaEffect = true;

        [ShowIf("_hasAreaEffect"), Button]
        private void RemoveAreaEffect()
        {
            _hasAreaEffect = false;
            _areaEffects = Array.Empty<AbilityEffect>();
            _areaRadius = 0;
        }

        [HideIf("@!_hasTarget || _hasTargetEffect"), Button]
        private void AddTargetEffect() => _hasTargetEffect = true;

        [ShowIf("@_hasTarget && _hasTargetEffect"), Button]
        private void RemoveTargetEffect()
        {
            _hasTargetEffect = false;
            _targetEffects = Array.Empty<AbilityEffect>();
        }
    }
}