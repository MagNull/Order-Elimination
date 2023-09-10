using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public interface IEffectData
    {
        public IReadOnlyEffectView View { get; }
        public EffectCharacter EffectCharacter { get; }
        public EffectStackingPolicy StackingPolicy { get; }
        public int MaxStackSize { get; }
        public bool UseApplierProcessing { get; }
        public bool UseHolderProcessing { get; }
        //RemoveTriggers
        public IEnumerable<EffectTriggerAcceptor> RemoveTriggers { get; }
        public IEffectInstruction InstructionOnActivation { get; }
        public IEffectInstruction InstructionOnDeactivation { get; }
        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; }
        public IReadOnlyDictionary<EffectTriggerAcceptor, IEffectInstruction> TriggerInstructions { get; }
        public IActionProcessor IncomingActionProcessor { get; } //CompoundProcessor
        public IActionProcessor OutcomingActionProcessor { get; }

        public EffectApplyResult CanBeAppliedOn(IEffectHolder effectHolder)
        {
            if (effectHolder.EffectImmunities.Contains(this))
                return EffectApplyResult.BlockedByImmunity;
            if (!effectHolder.HasEffect(this))
                return EffectApplyResult.Success;
            return StackingPolicy switch
            {
                EffectStackingPolicy.UnlimitedStacking => EffectApplyResult.Success,
                EffectStackingPolicy.OverrideOld => EffectApplyResult.Success,
                EffectStackingPolicy.IgnoreNew => EffectApplyResult.BlockedByStackingRules,
                EffectStackingPolicy.LimitedStacking => effectHolder.GetEffects(this).Length < MaxStackSize
                ? EffectApplyResult.Success : EffectApplyResult.BlockedByStackingLimit,
                _ => throw new System.NotImplementedException(),
            };
        }

        public void OnActivation(BattleEffect effect); //For extending functionality without EffectInstructions (not used)
        public void OnDeactivation(BattleEffect effect); //For extending functionality without EffectInstructions (not used)
    }
}
