using Cysharp.Threading.Tasks;
using System;
using static UnityEngine.Application;

namespace OrderElimination.AbilitySystem
{
    public abstract class BattleAction<TAction> : IBattleAction 
        where TAction : BattleAction<TAction> //TODO: reconsider necessity of this
    {
        public abstract ActionRequires ActionRequires { get; }

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
            if (ActionRequires == ActionRequires.Target)
            {
                if (useContext.ActionTarget == null)
                    throw new ArgumentNullException("Attempt to perform action on null entity.");
                if (useContext.ActionTarget.IsDisposedFromBattle)
                    throw new InvalidOperationException("Attempt to perform action on entity that had been disposed.");
            }
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            //modifiedAction.Callbacks += onCallback;
            var performResult = await modifiedAction.Perform(useContext);
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
