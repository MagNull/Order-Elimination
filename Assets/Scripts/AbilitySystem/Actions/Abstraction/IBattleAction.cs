using OrderElimination.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //public interface IModifiableAction<TAction> where TAction : IBattleAction
    //{
    //    public TAction GetModifiedAction(ActionUseContext useContext);
    //}

    [Obsolete("Интерфейс " + nameof(IBattleAction) + " является обобщающим. По возможности используйте BattleAction<TAction>.")]
    public interface IBattleAction
    {
        public event Action<IBattleAction> SuccessfullyPerformed;
        public event Action<IBattleAction> FailedToPerformed;
        public bool ModifiedPerform(ActionUseContext useContext);
    }

    public abstract class BattleAction<TAction> : IBattleAction where TAction : BattleAction<TAction>
    {
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

        public virtual TAction GetModifiedAction(ActionUseContext useContext)
        {
            var modifiedAction = (TAction)this;
            modifiedAction = useContext.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction);
            modifiedAction = useContext.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction);
            return modifiedAction;
        }

        public bool ModifiedPerform(ActionUseContext useContext)
        {
            var modifiedAction = GetModifiedAction(useContext);
            var actionIsPerformed = modifiedAction.Perform(useContext);
            if (actionIsPerformed)
                SuccessfullyPerformed?.Invoke(modifiedAction);
            else
                FailedToPerformed?.Invoke(modifiedAction);
            return actionIsPerformed;
        }

        //*При вызове Perform IBattleAction уже обработан.
        //Добавить out-параметр, дающий причину, по которой действие не было выполнено?
        protected abstract bool Perform(ActionUseContext useContext);

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
