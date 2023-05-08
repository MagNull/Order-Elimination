using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public enum EffectActionTarget
    {
        EffectHolder,
        EffectApplier
    }

    public interface IEffectInstruction
    {
        public void Execute(BattleEffect effect);
    }

    public class EffectInstruction : IEffectInstruction
    {
        [EnumToggleButtons]
        [ShowInInspector, OdinSerialize]
        private EffectActionTarget _target { get; set; } = EffectActionTarget.EffectHolder;

        [ValidateInput(
            "@!(" + nameof(_battleAction) + " is IUtilizeCellGroupsAction)",
            "Target group handling is not implemented for effects yet.")]
        [GUIColor(1f, 1, 0.2f)]
        [ShowInInspector, OdinSerialize]
        private IBattleAction _battleAction { get; set; }

        public void Execute(BattleEffect effect)
        {
            var cellGroups = new CellGroupsContainer();
            var target = _target switch
            {
                EffectActionTarget.EffectHolder => effect.EffectHolder,
                EffectActionTarget.EffectApplier => effect.EffectApplier,
                _ => throw new System.NotImplementedException(),
            };
            var actionContext = new ActionContext(
                effect.BattleContext, cellGroups, effect.EffectApplier, target);
            var applierProcessing = effect.EffectData.UseApplierProcessing;
            var holderProcessing = effect.EffectData.UseHolderProcessing;
            _battleAction.ModifiedPerform(actionContext, applierProcessing, holderProcessing);
        }
    }

    public class CompoundEffectInstruction : IEffectInstruction
    {
        [ShowInInspector, OdinSerialize]
        private List<IEffectInstruction> _instructions = new();

        public void Execute(BattleEffect effect)
        {
            foreach (var instruction in _instructions)
            {
                instruction.Execute(effect);
            }
        }
    }
}
