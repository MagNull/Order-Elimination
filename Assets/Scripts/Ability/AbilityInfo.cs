using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Ability
{
    public enum AbilityEffectType
    {
        Damage,
        Heal,
        Move,
        Buff
    }

    [Serializable]
    public struct AbilityEffect
    {
        public AbilityEffectType Type;
        public int Value;
    }

    [CreateAssetMenu(fileName = "Ability", menuName = "Ability/Ability Info")]
    public class AbilityInfo : SerializedScriptableObject
    {
        [Header("General Parameters")]
        [SerializeField]
        private float _coolDown;

        [field: SerializeField] public float StartCoolDown { get; private set; }

        [field: SerializeField] public Sprite Icon { get; private set; }

        #region Params

        [Header("Type Specific Parameters")]
        private bool _hasTarget;
        private bool _hasTargetEffect;

        [ShowIf("_hasTarget")]
        [SerializeField]
        private int _distance;

        [ShowIf("_hasTargetEffect")]
        [SerializeField]
        private AbilityEffect[] _targetEffects;

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

        public int Distance => _distance;

        public AbilityEffect[] TargetEffects => _targetEffects;

        public int AreaRadius => _areaRadius;

        public AbilityEffect[] AreaEffects => _areaEffects;

        public bool HasAreaEffect => _hasAreaEffect;

        public bool HasTargetEffect => _hasTargetEffect;

        #endregion

        [HideIf("_hasTarget"), Button]
        private void AddTarget() => _hasTarget = true;

        [ShowIf("_hasTarget"), Button]
        private void RemoveTarget() => _hasTarget = false;

        [HideIf("_hasAreaEffect"), Button]
        private void AddAreaEffect() => _hasAreaEffect = true;

        [ShowIf("_hasAreaEffect"), Button]
        private void RemoveAreaEffect() => _hasAreaEffect = false;
        
        [HideIf("_hasTargetEffect"), Button]
        private void AddTargetEffect() => _hasTargetEffect = true;
        
        [ShowIf("_hasTargetEffect"), Button]
        private void RemoveTargetEffect() => _hasTargetEffect = false;
    }
}