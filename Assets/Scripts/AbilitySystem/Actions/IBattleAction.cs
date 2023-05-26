using Cysharp.Threading.Tasks;
using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public enum ActionRequires
    {
        Entity,
        Cell,
        Nothing
    }

    [Obsolete("��������� " + nameof(IBattleAction) + " �������� ����������. �� ����������� ����������� BattleAction<TAction>.")]
    public interface IBattleAction : ICloneable<IBattleAction>
    {
        public ActionRequires ActionRequires { get; }

        //public bool CanPerform(ActionExecutionContext useContext, bool actionMakerProcessing = true, bool targetProcessing = true);

        public event Action<IActionPerformResult> SuccessfullyPerformed;
        public event Action<IActionPerformResult> FailedToPerformed;
        //public int RepeatNumber
        public UniTask<IActionPerformResult> ModifiedPerform(
            ActionContext useContext, 
            bool actionMakerProcessing = true,
            bool targetProcessing = true);
    }

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

        public abstract IBattleAction Clone();
    }
}
