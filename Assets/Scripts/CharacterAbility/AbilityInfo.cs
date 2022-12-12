using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OrderElimination;
using Sirenix.OdinInspector;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.Serialization;

namespace CharacterAbility
{
    public enum AbilityEffectType
    {
        Damage,
        Heal,
        Move,
        OverTime,
        TickingBuff,
        ConditionalBuff,
        Modificator,
        Stun,
        Contreffect
    }

    public enum BuffConditionType
    {
        Moved,
        Damaged
    }

    public enum AbilityScaleFrom
    {
        Attack,
        Health,
        Movement,
        Distance
    }

    public enum OverTimeAbilityType
    {
        Damage,
        Heal,
    }

    public enum Buff_Type
    {
        Attack,
        Health,
        Accuracy,
        Movement,
        Evasion, //%
        IncomingDamageIncrease,
        IncomingAccuracy, //%
        IncomingDamageReduction
    }

    public enum ModificatorType
    {
        Accuracy,
        DoubleArmorDamage,
        DoubleHealthDamage
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
        [ShowIf("@Type != AbilityEffectType.Modificator")]
        public bool HasProbability;
        [ShowIf("HasProbability")]
        public float Probability;
        [ShowIf("@Type != AbilityEffectType.Move")]
        public BattleObjectSide Filter;
        [ShowIf(
            "@Type == AbilityEffectType.Damage || Type == AbilityEffectType.Heal || Type == AbilityEffectType.OverTime")]
        public DamageHealTarget _damageHealTarget;
        [ShowIf("@Type == AbilityEffectType.Damage || Type == AbilityEffectType.Heal")]
        public int Amounts;
        [ShowIf("@Type == AbilityEffectType.Damage || Type == AbilityEffectType.Heal")]
        public AbilityScaleFrom ScaleFrom;
        [ShowIf("@Type == AbilityEffectType.Damage || Type == AbilityEffectType.Heal")]
        public float Scale;

        [ShowIf("@Type == AbilityEffectType.Modificator")]
        public ModificatorType Modificator;
        [ShowIf("@Type == AbilityEffectType.Modificator && Modificator == ModificatorType.Accuracy")]
        public int ModificatorValue;

        [ShowIf("@Type == AbilityEffectType.TickingBuff || Type == AbilityEffectType.ConditionalBuff")]
        public Buff_Type BuffType;
        [ShowIf("@Type == AbilityEffectType.TickingBuff || Type == AbilityEffectType.ConditionalBuff")]
        public int BuffValue;
        [ShowIf("@Type == AbilityEffectType.OverTime || Type == AbilityEffectType.TickingBuff")]
        public int Duration;
        [ShowIf("@Type == AbilityEffectType.ConditionalBuff")]
        public BuffConditionType ConditionType;

        [ShowIf("@Type == AbilityEffectType.OverTime")]
        public OverTimeAbilityType OverTimeType;
        [ShowIf("@Type == AbilityEffectType.OverTime")]
        public int TickValue;

        [ShowIf(
            "@Type == AbilityEffectType.Damage || " +
            "(Type == AbilityEffectType.OverTime && OverTimeType == OverTimeAbilityType.Damage)" +
            "|| ((Type == AbilityEffectType.TickingBuff || Type == AbilityEffectType.ConditionalBuff) && " +
            "(BuffType == Buff_Type.IncomingDamageIncrease || BuffType == Buff_Type.IncomingDamageReduction ||" +
            " BuffType == Buff_Type.IncomingAccuracy))")]
        public DamageType DamageType;

        [ShowIf("@Type == AbilityEffectType.Contreffect")]
        public int Distance;

        [Title("Description Flag")]
        [TextArea]
        public string DescriptionFlag;

        public bool ShowInAbilityDescription;
        [ShowIf("@" + nameof(ShowInAbilityDescription) + " == true")]
        public EffectView EffectView;
    }

    [CreateAssetMenu(fileName = "AbilityInfo", menuName = "Ability")]
    public class AbilityInfo : SerializedScriptableObject
    {
        public enum AbilityType
        {
            Active,
            Passive
        }

        [Title("General Parameters")]
        [SerializeField]
        private string _name;
        [SerializeField]
        [TextArea]
        private string _description;
        [SerializeField]
        [Range(0, 10)]
        private int _coolDown;
        [SerializeField]
        [Range(0, 10)]
        private int _startCoolDown;

        [field: SerializeField] public Sprite Icon { get; private set; }

        [field: SerializeField] public ActionType ActionType { get; private set; }

        [SerializeField]
        private AbilityType _type;

        [ShowIf("@_type == AbilityType.Active")]
        [SerializeField]
        private ActiveAbilityParams _activeParams;

        [ShowIf("@_type == AbilityType.Passive")]
        [SerializeField]
        private PassiveAbilityParams _passiveParams;

        public int CoolDown => _coolDown + 1;

        public int StartCoolDown => _startCoolDown + 1;

        public string Name => _name;

        public string Description => _description;

        public ActiveAbilityParams ActiveParams => _activeParams;

        public AbilityType Type => _type;

        public PassiveAbilityParams PassiveParams => _passiveParams;

        public AbilityEffect GetTargetEffectByFlag(string flag) =>
            _activeParams.TargetEffects.First(x => x.DescriptionFlag == flag);

        public AbilityEffect GetAreaEffectByFlag(string flag) =>
            _activeParams.AreaEffects.First(x => x.DescriptionFlag == flag);

        private void OnValidate()
        {
            if (_type == AbilityType.Active)
                _passiveParams = default;
            else _activeParams = default;
        }
    }

    [Serializable]
    public struct ActiveAbilityParams
    {
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

        public bool HasTarget => _hasTarget;
        public TargetType TargetType => _targetType;
        public int Distance => _distance;

        public AbilityEffect[] TargetEffects => _targetEffects;

        public int AreaRadius => _areaRadius;

        public AbilityEffect[] AreaEffects => _areaEffects;

        public bool HasAreaEffect => _hasAreaEffect;

        public bool HasTargetEffect => _hasTargetEffect;

        public bool DistanceFromMovement => _distanceFromMovement;

        #endregion

        #region Buttons

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

        #endregion
    }

    [Serializable]
    public struct PassiveAbilityParams
    {
        public enum PassiveTriggerType
        {
            Spawn,
            Movement,
            Damage
        }

        [SerializeField]
        private AbilityEffect[] _effects;
        [SerializeField]
        private PassiveTriggerType _triggerType;

        public AbilityEffect[] Effects => _effects;

        public PassiveTriggerType TriggerType => _triggerType;
    }
}