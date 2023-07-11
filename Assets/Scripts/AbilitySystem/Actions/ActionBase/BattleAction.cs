using Cysharp.Threading.Tasks;
using System;
using System.Diagnostics;

namespace OrderElimination.AbilitySystem
{
    public abstract class BattleAction<TAction> : IBattleAction 
        where TAction : BattleAction<TAction>
    {
        public abstract ActionRequires ActionRequires { get; }

        public event Action<IActionPerformResult> SuccessfullyPerformed;
        public event Action<IActionPerformResult> FailedToPerformed;

        public TAction GetModifiedAction(
            ActionContext useContext,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            var modifyingAction = (TAction)this.Clone();
            return modifyingAction.ProcessAction(useContext, actionMakerProcessing, targetProcessing);
        }

        public async UniTask<IActionPerformResult> ModifiedPerform(
            ActionContext useContext,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            //TODO: Refactor. "Entity disposed" case shouldn't be reached.
            if (ActionRequires == ActionRequires.Entity && useContext.ActionTarget.IsDisposedFromBattle)
            {
                Logging.LogError("Attempt to perform action on entity that had been disposed.");
                return new SimplePerformResult(this, useContext, false);
            }
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            var performResult = await modifiedAction.Perform(useContext);
            if (performResult.IsSuccessful)
                SuccessfullyPerformed?.Invoke(performResult);
            else
                FailedToPerformed?.Invoke(performResult);
            return performResult;
        }

        protected virtual TAction ProcessAction(
            ActionContext performContext, 
            bool actionMakerProcessing = true, 
            bool targetProcessing = true)
        {
            var modifiedAction = (TAction)this;
            if (actionMakerProcessing && performContext.ActionMaker != null)
                modifiedAction = performContext.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction, performContext);
            if (targetProcessing && performContext.ActionTarget != null)
                modifiedAction = performContext.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction, performContext);
            return modifiedAction;
        }

        protected abstract UniTask<IActionPerformResult> Perform(ActionContext useContext);

        //protected abstract IActionPerformResult GetOnFailResult();

        public abstract IBattleAction Clone();
    }
}
