using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ApplyTemporaryArmorAction : BattleAction<ApplyTemporaryArmorAction>, 
        IUndoableBattleAction,
        ICallbackingBattleAction
    {
        private static readonly List<TemporaryArmor> _appliedTempArmors = new();
        private static readonly List<IUndoableActionPerformResult> _actionResults = new();
        private static readonly HashSet<int> _undoneOperations = new();

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter TemporaryArmorAmount { get; private set; }

        public override ActionRequires ActionRequires => ActionRequires.Target;

        public string CallbackDescription => "Callback happens when temporary armor is depleted.";

        protected event Action<IBattleActionCallback> Callbacks;

        public void ClearUndoCache()
        {
            _undoneOperations.Clear();
            _appliedTempArmors.Clear();
            _actionResults.Clear();
        }

        public override IBattleAction Clone()
        {
            var clone = new ApplyTemporaryArmorAction();
            clone.TemporaryArmorAmount = TemporaryArmorAmount.Clone();
            clone.Callbacks = Callbacks;//dont copy callbacks?
            return clone;
        }

        public bool IsUndone(int performId)
        {
            return _undoneOperations.Contains(performId);
        }

        public bool Undo(int performId)
        {
            if (_undoneOperations.Contains(performId))
            {
                Logging.LogException(ActionUndoFailedException.AlreadyUndoneException);
                return false;
            }
            var result = _actionResults[performId];
            if (!result.IsSuccessful) 
                return false;
            var armor = _appliedTempArmors[performId];
            result.ActionContext.ActionTarget.BattleStats.RemoveTemporaryArmor(armor);
            _undoneOperations.Add(performId);
            return true;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var calculationContext = ValueCalculationContext.Full(useContext);
            var temporaryArmor = new TemporaryArmor(TemporaryArmorAmount.GetValue(calculationContext));
            var currentId = _appliedTempArmors.Count;
            useContext.ActionTarget.BattleStats.TemporaryArmorLayerRemoved += OnTemporaryArmorRemoved;
            useContext.ActionTarget.BattleStats.AddTemporaryArmor(temporaryArmor);
            var actionResult = new SimpleUndoablePerformResult(this, useContext, true, currentId);
            _appliedTempArmors.Add(temporaryArmor);
            _actionResults.Add(actionResult);
            return actionResult;

            void OnTemporaryArmorRemoved(TemporaryArmor armor)
            {
                if (armor == temporaryArmor)
                {
                    useContext.ActionTarget.BattleStats.TemporaryArmorLayerRemoved -= OnTemporaryArmorRemoved;
                    Callbacks?.Invoke(new ApplyTemporaryArmorActionCallback(armor));
                }
            }
        }

        public async UniTask<IActionPerformResult> ModifiedPerformWithCallbacks(
            ActionContext useContext,
            Action<IBattleActionCallback> onCallback,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            if (ActionRequires == ActionRequires.Target)
            {
                if (useContext.ActionTarget == null)
                    throw new ArgumentNullException("Attempt to perform action on null entity.");
                if (useContext.ActionTarget.IsDisposedFromBattle)
                    throw new InvalidOperationException("Attempt to perform action on entity that had been disposed.");
            }
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            modifiedAction.Callbacks += onCallback;
            var performResult = await modifiedAction.Perform(useContext);
            return performResult;
        }
    }
}
