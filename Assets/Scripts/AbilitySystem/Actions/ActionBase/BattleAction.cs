using Cysharp.Threading.Tasks;
using System;
using static UnityEngine.Application;

namespace OrderElimination.AbilitySystem
{
    public abstract class BattleAction<TAction> : IBattleAction 
        where TAction : BattleAction<TAction> //TODO: reconsider necessity of this
    {
        public abstract BattleActionType BattleActionType { get; }

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
            if (BattleActionType == BattleActionType.EntityAction)
            {
                if (useContext.TargetEntity == null)
                    throw new ArgumentNullException("Attempt to perform action on null entity.");
                if (useContext.TargetEntity.IsDisposedFromBattle)
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
            if (targetProcessing && performContext.TargetEntity != null)
                modifiedAction = performContext.TargetEntity.ActionProcessor.ProcessIncomingAction(modifiedAction, performContext);
            return modifiedAction;
        }

        protected abstract UniTask<IActionPerformResult> Perform(ActionContext useContext);

        //protected abstract IActionPerformResult GetOnFailResult();

        public abstract IBattleAction Clone();
    }
}
