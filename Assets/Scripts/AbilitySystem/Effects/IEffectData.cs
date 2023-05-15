using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface IEffectData
    {
        public IReadOnlyEffectView View { get; }
        public EffectStackingPolicy StackingPolicy { get; }
        public bool UseApplierProcessing { get; }
        public bool UseHolderProcessing { get; }
        public bool CanBeForceRemoved { get; }
        //RemoveTriggers

        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; }
        public IReadOnlyDictionary<ITriggerSetupInfo, IEffectInstruction> TriggerInstructions { get; }
        public IActionProcessor IncomingActionProcessor { get; } //CompoundProcessor
        public IActionProcessor OutcomingActionProcessor { get; }

        public bool CanBeAppliedOn(IEffectHolder effectHolder)
            => !effectHolder.HasEffect(this) || StackingPolicy != EffectStackingPolicy.IgnoreNew;

        public void OnActivation(BattleEffect effect);
        public void OnDeactivation(BattleEffect effect);
    }
}
