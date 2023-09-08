using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EffectActionInstruction : IEffectInstruction
    {
        #region OdinVisuals
        private bool ActionIsUndoable => _battleAction is IUndoableBattleAction;
        private bool IsEntityAction => _battleAction != null ? _battleAction.ActionRequires == ActionRequires.Target : false;
        private bool IsCallbackingAction => _battleAction is ICallbackingBattleAction;

        [OnInspectorInit]
        private void ActionUndoableValidate()
        {
            if (!ActionIsUndoable)
            {
                UndoOnDeactivation = false;
            }
        }
        #endregion

        [BoxGroup("Action", ShowLabel = false)]
        [OnValueChanged("@" + nameof(ActionUndoableValidate) + "()")]
        [GUIColor(1f, 1, 0.2f)]
        [ShowInInspector, OdinSerialize]
        private IBattleAction _battleAction { get; set; }

        [BoxGroup("Action", ShowLabel = false)]
        [EnableIf("@" + nameof(IsEntityAction))]
        [ShowInInspector, OdinSerialize]
        private EffectEntity _target { get; set; } = EffectEntity.EffectHolder;

        [BoxGroup("Action", ShowLabel = false)]
        [EnableIf("@" + nameof(ActionIsUndoable))]
        [PropertyTooltip("@(" + nameof(ActionIsUndoable) + " ? \"\" : \"Action is not Undoable\")")]
        [ValidateInput("@" + nameof(UndoOnDeactivation) + " || !" + nameof(ActionIsUndoable), "Action will never be undone! Are you sure this is right?")]
        [ShowInInspector, OdinSerialize]
        private bool UndoOnDeactivation { get; set; } = true;

        //UndoTriggers
        [BoxGroup("Next Instruction", ShowLabel = false)]
        [GUIColor(1f, 1f, 0.3f)]
        [EnableIf("@" + nameof(IsCallbackingAction))]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _instructionOnCallback { get; set; }

        [BoxGroup("Next Instruction", ShowLabel = false)]
        [GUIColor(0.7f, 1f, 0.7f)]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _instructionOnSuccess { get; set; }

        [BoxGroup("Next Instruction", ShowLabel = false)]
        [GUIColor(1f, 0.7f, 0.7f)]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _instructionOnFail { get; set; }

        [BoxGroup("Next Instruction", ShowLabel = false)]
        [GUIColor(0.75f, 0.75f, 1f)]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _followingInstruction { get; set; }

        [BoxGroup("Animations", ShowLabel = false)]
        [GUIColor(0, 1, 1)]
        [ShowInInspector, OdinSerialize]
        private IAbilityAnimation _animationBeforeAction { get; set; }

        [BoxGroup("Animations", ShowLabel = false)]
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

            if (_battleAction == null)
            {
                var entitiesBank = effect.BattleContext.EntitiesBank;
                Logging.LogError(
                    $"Executing instruction with null action on effect {effect.EffectData.View.Name}\n"
                    + $"Holder: {entitiesBank.GetViewByEntity(effect.EffectHolder).Name}\n"
                    + $"Applier: {entitiesBank.GetViewByEntity(effect.EffectApplier).Name}\n");
            }
            var result = _battleAction is ICallbackingBattleAction callbackingAction
                ? await callbackingAction.ModifiedPerformWithCallbacks(actionContext, OnCallback, applierProcessing, holderProcessing)
                : await _battleAction.ModifiedPerform(actionContext, applierProcessing, holderProcessing);
            effect.Deactivated += OnDeactivation;

            if (result.IsSuccessful && _instructionOnSuccess != null)
            {
                await _instructionOnSuccess.Execute(effect);
            }
            if (!result.IsSuccessful && _instructionOnFail != null)
            {
                await _instructionOnFail.Execute(effect);
            }
            if (_followingInstruction != null)
                await _followingInstruction.Execute(effect);

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

            void OnCallback(IBattleActionCallback callback)
            {
                if (_instructionOnCallback != null)
                    _instructionOnCallback.Execute(effect);//no await?
            }
        }
    }
}
