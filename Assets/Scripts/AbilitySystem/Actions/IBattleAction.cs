using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public enum ActionRequires
    {
        Entity,
        Cell,
        Nothing
    }

    public enum ActionPerformEntity
    {
        Caster,
        Target
    }

    [Obsolete("Интерфейс " + nameof(IBattleAction) + " является обобщающим. По возможности используйте BattleAction<TAction>.")]
    public interface IBattleAction
    {
        public ActionRequires ActionExecutes { get; }

        //public bool CanPerform(ActionExecutionContext useContext, bool actionMakerProcessing = true, bool targetProcessing = true);

        public event Action<IBattleAction> SuccessfullyPerformed;
        public event Action<IBattleAction> FailedToPerformed;
        //public int RepeatNumber
        public UniTask<bool> ModifiedPerform(
            ActionContext useContext, 
            bool actionMakerProcessing = true,
            bool targetProcessing = true);
    }

    public interface IUtilizeCellGroupsAction : IBattleAction
    {
        public IEnumerable<int> UtilizingCellGroups { get; }

        public int GetUtilizedCellsAmount(int group);
    }

    public interface IPerformOnSelectedEntityAction
    {
        public ActionPerformEntity PerformEntity { get; }
    }

    public abstract class BattleAction<TAction> : IBattleAction where TAction : BattleAction<TAction>
    {
        public abstract ActionRequires ActionExecutes { get; }

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

        public async UniTask<bool> ModifiedPerform(
            ActionContext useContext, 
            bool actionMakerProcessing = true, 
            bool targetProcessing = true)
        {
            var modifiedAction = GetModifiedAction(useContext, actionMakerProcessing, targetProcessing);
            var actionIsPerformed = await modifiedAction.Perform(useContext);
            if (actionIsPerformed)
                SuccessfullyPerformed?.Invoke(modifiedAction);
            else
                FailedToPerformed?.Invoke(modifiedAction);
            return actionIsPerformed;
        }

        //*При вызове Perform IBattleAction уже обработан.
        //TODO Желательно заменить возврат bool или добавить out-параметр, дающий причину, по которой действие не было выполнено.
        protected abstract UniTask<bool> Perform(ActionContext useContext);

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
