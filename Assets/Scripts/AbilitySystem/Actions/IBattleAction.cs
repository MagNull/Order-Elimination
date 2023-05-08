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

    [Obsolete("Интерфейс " + nameof(IBattleAction) + " является обобщающим. По возможности используйте BattleAction<TAction>.")]
    public interface IBattleAction : ICloneable<IBattleAction>
    {
        public ActionRequires ActionRequires { get; }

        //public bool CanPerform(ActionExecutionContext useContext, bool actionMakerProcessing = true, bool targetProcessing = true);

        public event Action<IBattleAction, ActionContext> SuccessfullyPerformed;
        public event Action<IBattleAction, ActionContext> FailedToPerformed;
        //public int RepeatNumber
        public UniTask<bool> ModifiedPerform(
            ActionContext useContext, 
            bool actionMakerProcessing = true,
            bool targetProcessing = true);
    }

    public abstract class BattleAction<TAction> : IBattleAction where TAction : BattleAction<TAction>
    {
        public abstract ActionRequires ActionRequires { get; }

        public event Action<TAction, ActionContext> SuccessfullyPerformed;
        public event Action<TAction, ActionContext> FailedToPerformed;

        event Action<IBattleAction, ActionContext> IBattleAction.SuccessfullyPerformed
        {
            add => SuccessfullyPerformed += value;

            remove => SuccessfullyPerformed -= value;
        }

        event Action<IBattleAction, ActionContext> IBattleAction.FailedToPerformed
        {
            add => FailedToPerformed += value;

            remove => FailedToPerformed -= value;
        }

        public TAction GetModifiedAction(
            ActionContext useContext,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            var modifyingAction = (TAction)this.Clone();
            return modifyingAction.ModifyAction(useContext, actionMakerProcessing, targetProcessing);
        }

        public async UniTask<bool> ModifiedPerform(
            ActionContext useContext,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            var actionIsPerformed = await modifiedAction.Perform(useContext);
            if (actionIsPerformed)
                SuccessfullyPerformed?.Invoke(modifiedAction, useContext);
            else
                FailedToPerformed?.Invoke(modifiedAction, useContext);
            return actionIsPerformed;
        }

        protected virtual TAction ModifyAction(
            ActionContext useContext, 
            bool actionMakerProcessing = true, 
            bool targetProcessing = true)
        {
            var modifiedAction = (TAction)this;
            if (actionMakerProcessing && useContext.ActionMaker != null)
                modifiedAction = useContext.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction);
            if (targetProcessing && useContext.ActionTarget != null)
                modifiedAction = useContext.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction);
            return modifiedAction;
        }

        //TODO Желательно заменить возврат bool или добавить out-параметр, дающий причину, по которой действие не было выполнено.
        protected abstract UniTask<bool> Perform(ActionContext useContext);

        public abstract IBattleAction Clone();
    }
}
