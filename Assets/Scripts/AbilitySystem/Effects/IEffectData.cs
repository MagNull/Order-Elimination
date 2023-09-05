using System.Collections.Generic;

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

        public bool CanBeAppliedOn(IEffectHolder effectHolder)
        {
            if (!effectHolder.HasEffect(this))
                return true;
            return StackingPolicy switch
            {
                EffectStackingPolicy.UnlimitedStacking => true,
                EffectStackingPolicy.OverrideOld => true,
                EffectStackingPolicy.IgnoreNew => false,
                EffectStackingPolicy.LimitedStacking => effectHolder.GetEffects(this).Length < MaxStackSize,
                _ => throw new System.NotImplementedException(),
            };
        }

        public void OnActivation(BattleEffect effect); //For extending functionality without EffectInstructions (not used)
        public void OnDeactivation(BattleEffect effect); //For extending functionality without EffectInstructions (not used)
    }
}
