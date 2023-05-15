using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
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
        public UniTask Execute(BattleEffect effect);
    }

    public class EffectInstruction : IEffectInstruction
    {
        [EnumToggleButtons]
        [ShowInInspector, OdinSerialize]
        private EffectActionTarget _target { get; set; } = EffectActionTarget.EffectHolder;

        [GUIColor(1f, 1, 0.2f)]
        [ShowInInspector, OdinSerialize]
        private IBattleAction _battleAction { get; set; }

        [ShowIf("@" + nameof(_battleAction) + " is " + nameof(IUndoableBattleAction))]
        [ValidateInput("@" + nameof(UndoOnDeactivation), "Action will never be undone! Are you sure this is right?")]
        [ShowInInspector, OdinSerialize]
        private bool UndoOnDeactivation { get; set; } = true;

        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        private IAbilityAnimation _animationBeforeAction { get; set; }

        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        private IAbilityAnimation _animationAfterAction { get; set; }

        public async UniTask Execute(BattleEffect effect)
        {
            var cellGroups = CellGroupsContainer.Empty;
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
            var animationContext = new AnimationPlayContext(
                effect.BattleContext.AnimationSceneContext, cellGroups, effect.EffectApplier, effect.EffectHolder);
            if (_animationBeforeAction != null)
                await _animationBeforeAction.Play(animationContext);
            var result = await _battleAction.ModifiedPerform(actionContext, applierProcessing, holderProcessing);
            effect.Deactivated += OnDeactivation;
            if (_animationAfterAction != null)
                await _animationAfterAction.Play(animationContext);

            void OnDeactivation(BattleEffect effect)
            {
                effect.Deactivated -= OnDeactivation;
                if (result.ModifiedAction is IUndoableBattleAction undoableAction
                    && UndoOnDeactivation)
                {
                    var undoableActionResult = (IUndoableActionPerformResult)result;
                    undoableAction.Undo(undoableActionResult.PerformId);
                }
            }
        }
    }

    public class CompoundEffectInstruction : IEffectInstruction
    {
        [ShowInInspector, OdinSerialize]
        private List<IEffectInstruction> _instructions = new();

        public async UniTask Execute(BattleEffect effect)
        {
            foreach (var instruction in _instructions)
            {
                await instruction.Execute(effect);
            }
        }
    }
}
