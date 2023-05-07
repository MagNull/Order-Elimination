using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public enum EffectStackingPolicy
    {
        UnlimitedStacking,
        IgnoreNew,
        OverrideOld,
    }

    public class TemporaryEffectFunctionaity
    {
        [HideInInspector, OdinSerialize]
        private int _applyingDuration = 1;

        [ShowInInspector]
        public int ApplyingDuration
        {
            get => _applyingDuration;
            set
            {
                if (value < 1) value = 1;
                _applyingDuration = value;
            }
        }

        public virtual void OnTimeOut(BattleEffect effect)
        {

        }
    }

    public interface IIncomingActionEffectProcessor
    {
        public IBattleAction ProcessIncomingAction(IBattleAction originalAction);
    }

    public interface IOutcomingActionEffectProcessor
    {
        public IBattleAction ProcessOutcomingAction(IBattleAction originalAction);
    }

    public interface IEffectData
    {
        public EffectView View { get; }
        public EffectStackingPolicy StackingPolicy { get; }
        public bool UseApplierProcessing { get; }
        public bool UseHolderProcessing { get; }
        public bool CanBeForceRemoved { get; }
        //RemoveTriggers

        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; }
        public IIncomingActionEffectProcessor IncomingProcessor { get; } //CompoundProcessor
        public IOutcomingActionEffectProcessor OutcomingProcessor { get; }

        public bool CanBeAppliedOn(IEffectHolder effectHolder)
            => !effectHolder.HasEffect(this) || StackingPolicy != EffectStackingPolicy.IgnoreNew;

        public void OnActivation(BattleEffect effect);
        public void OnDeactivation(BattleEffect effect);
    }

    [CreateAssetMenu(fileName = "EffectPreset", menuName = "AbilitySystem/Effect")]
    public class EffectDataPreset : SerializedScriptableObject, IEffectData
    {
        [ShowInInspector, OdinSerialize]
        public EffectView View { get; protected set; }

        [ShowInInspector, OdinSerialize]
        public EffectStackingPolicy StackingPolicy { get; protected set; }

        [ShowInInspector, OdinSerialize]
        public bool UseApplierProcessing { get; protected set; }

        [ShowInInspector, OdinSerialize]
        public bool UseHolderProcessing { get; protected set; }

        [ShowInInspector, OdinSerialize]
        public bool CanBeForceRemoved { get; protected set; }

        #region Optionals
        [ShowInInspector, OdinSerialize]
        public bool IsTemporary { get; protected set; }

        [ShowIf("@" + nameof(IsTemporary))]
        [ShowInInspector, OdinSerialize]
        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; protected set; }

        [ShowInInspector, OdinSerialize]
        public bool IsProcessingIncomingAction { get; protected set; }
        
        [ShowIf("@" + nameof(IsProcessingIncomingAction))]
        [ShowInInspector, OdinSerialize]
        public IIncomingActionEffectProcessor IncomingProcessor { get; protected set; }

        [ShowInInspector, OdinSerialize]
        public bool IsProcessingOutcomingAction { get; protected set; }

        [ShowIf("@" + nameof(IsProcessingOutcomingAction))]
        [ShowInInspector, OdinSerialize]
        public IOutcomingActionEffectProcessor OutcomingProcessor { get; protected set; }
        #endregion

        public virtual void OnActivation(BattleEffect effect)
        {

        }

        public void OnDeactivation(BattleEffect effect)
        {

        }
    }
}
