﻿using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class EffectActionInstruction : IEffectInstruction
    {
        #region OdinVisuals
        private bool ActionIsUndoable => _battleAction is IUndoableBattleAction;

        [OnInspectorInit]
        private void ActionUndoableValidate()
        {
            if (!ActionIsUndoable)
            {
                UndoOnDeactivation = false;
            }
        }
        #endregion

        [ShowInInspector, OdinSerialize]
        private EffectEntity _target { get; set; } = EffectEntity.EffectHolder;

        [OnValueChanged("@" + nameof(ActionUndoableValidate) + "()")]
        [GUIColor(1f, 1, 0.2f)]
        [ShowInInspector, OdinSerialize]
        private IBattleAction _battleAction { get; set; }

        [EnableIf("@" + nameof(ActionIsUndoable))]
        [PropertyTooltip("@(" + nameof(ActionIsUndoable) + " ? \"\" : \"Action is not Undoable\")")]
        [ValidateInput("@" + nameof(UndoOnDeactivation) + " || !" + nameof(ActionIsUndoable), "Action will never be undone! Are you sure this is right?")]
        [ShowInInspector, OdinSerialize]
        private bool UndoOnDeactivation { get; set; } = true;

        //UndoTriggers

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
                EffectEntity.EffectHolder => effect.EffectHolder,
                EffectEntity.EffectApplier => effect.EffectApplier,
                _ => throw new NotImplementedException(),
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
}
