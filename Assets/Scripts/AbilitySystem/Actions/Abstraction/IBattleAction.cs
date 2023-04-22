using System;

namespace OrderElimination.AbilitySystem
{
    public enum ActionTargets
    {
        EntitiesOnly,
        CellsOnly
    }

    [Obsolete("Интерфейс " + nameof(IBattleAction) + " является обобщающим. По возможности используйте BattleAction<TAction>.")]
    public interface IBattleAction
    {
        public ActionTargets ActionTargets { get; }

        public event Action<IBattleAction> SuccessfullyPerformed;
        public event Action<IBattleAction> FailedToPerformed;
        //public int RepeatNumber
        public bool ModifiedPerform(
            ActionExecutionContext useContext, 
            bool actionMakerProcessing = true,
            bool targetProcessing = true);
    }

    public interface IUndoableBattleAction
    {
        public bool Undo(IAbilitySystemActor undoTarget);
    }

    public abstract class BattleAction<TAction> : IBattleAction where TAction : BattleAction<TAction>
    {
        public abstract ActionTargets ActionTargets { get; }

        public event Action<TAction> SuccessfullyPerformed;
        public event Action<TAction> FailedToPerformed;

        event Action<IBattleAction> IBattleAction.SuccessfullyPerformed
        {
            add => SuccessfullyPerformed += value;

            remove => SuccessfullyPerformed -= value;
        }

        event Action<IBattleAction> IBattleAction.FailedToPerformed
        {
            add => FailedToPerformed += value;

            remove => FailedToPerformed -= value;
        }

        public virtual TAction GetModifiedAction(
            ActionExecutionContext useContext, 
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

        public bool ModifiedPerform(
            ActionExecutionContext useContext, 
            bool actionMakerProcessing = true, 
            bool targetProcessing = true)
        {
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            var actionIsPerformed = modifiedAction.Perform(useContext);
            if (actionIsPerformed)
                SuccessfullyPerformed?.Invoke(modifiedAction);
            else
                FailedToPerformed?.Invoke(modifiedAction);
            return actionIsPerformed;
        }

        //*При вызове Perform IBattleAction уже обработан.
        //TODO Желательно заменить возврат bool или добавить out-параметр, дающий причину, по которой действие не было выполнено.
        protected abstract bool Perform(ActionExecutionContext useContext);

        //public void SubscribePerform(Action<IBattleAction> actionEvent)
        //{
        //    actionEvent -= OnSubscribeActionEventCall;
        //    actionEvent += OnSubscribeActionEventCall;
        //    void OnSubscribeActionEventCall(IBattleAction subscribingAction)
        //    {
        //        ModifiedPerform(useContext);
        //    }
        //}
    }
}
