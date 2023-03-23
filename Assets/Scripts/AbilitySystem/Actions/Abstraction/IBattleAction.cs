using OrderElimination.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleAction
    {
        //public event Action<IBattleAction> SuccessfullyPerformed;
        //public event Action<IBattleAction> FailedToPerformed;
    }

    public interface IBattleAction<TTarget> : IBattleAction where TTarget : IActionTarget
    {
        public event Action<IBattleAction<TTarget>> SuccessfullyPerformed;
        public event Action<IBattleAction<TTarget>> FailedToPerformed; // <-- Объяснить причину? // Возвращать ActionUseContext?

        //Добавить out-параметр, дающий причину, по которой действие не было выполнено?
        public bool Perform(ActionUseContext useContext, IBattleEntity actionMaker, TTarget target);

        public void SubscribePerform(Action<IBattleAction<IActionTarget>> actionEvent, ActionUseContext useContext, IBattleEntity actionMaker, TTarget target)
        {
            actionEvent -= OnSubscribeActionEventCall;
            actionEvent += OnSubscribeActionEventCall;
            void OnSubscribeActionEventCall(IBattleAction<IActionTarget> subscribingAction)
            {
                Perform(useContext, actionMaker, target);
            }
        }
    }

    public abstract class EntityBattleAction : IBattleAction<IBattleEntity>
    {
        public event Action<IBattleAction<IBattleEntity>> SuccessfullyPerformed;
        public event Action<IBattleAction<IBattleEntity>> FailedToPerformed;

        public abstract bool Perform(ActionUseContext useContext, IBattleEntity actionMaker, IBattleEntity target);
    }

    public abstract class CellBattleAction : IBattleAction<Cell>
    {
        public event Action<IBattleAction<Cell>> SuccessfullyPerformed;
        public event Action<IBattleAction<Cell>> FailedToPerformed;

        public abstract bool Perform(ActionUseContext useContext, IBattleEntity actionMaker, Cell target);
    }
}
