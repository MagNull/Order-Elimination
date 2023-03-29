using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public enum EffectCharacter
    {
        Positive,
        Negative,
        Neutral
    }

    public interface IEffect
    {
        public static bool IsStackable { get; }
        public static bool UseApplierActionProcessor { get; } //Использовать обработчик, наложившего эффект?
        public static IBattleAction[] ActionsOnApply { get; }
        public static IBattleAction[] ActionsOnRemove { get; }
        public IAbilitySystemActor EffectApplier { get; } 
        public IAbilitySystemActor EffectHolder { get; }
        //RemovedByTriggers
        public event Action<IEffect> Removed;//Destroyed Disposed Finished
    }

    public interface ITemporaryEffect : IEffect
    {
        public int Duration { get; }
        public event Action<ITemporaryEffect> EffectEnded;
    }

    public interface IIncomingActionProcessingEffect<TAction> : IEffect where TAction : BattleAction<TAction>
    {
        public TAction ProcessIncomingAction(TAction originalAction);
    }

    public interface IOutcomingActionProcessingEffect<TAction> : IEffect where TAction : BattleAction<TAction>
    {
        public TAction ProcessOutcomingAction(TAction originalAction);
    }

    public interface ITickActionEffect : IEffect
    {
        public int TickLength { get; } // 1Tick = 1 ход
        //Действия обязаны выполняться каждый промежуток ходов, пока активен эффект.
        //Действия должны прекратить выполняться при удалении эффекта.
        public IBattleAction[] ActionsPerTick { get; }
        public void OnTick(IBattleContext battleContext)
        {
            foreach (var action in ActionsPerTick)
            {
                var actionMaker = UseApplierActionProcessor ? EffectApplier : null;
                var actionContext = new ActionExecutionContext(battleContext, actionMaker, EffectHolder);
                action.ModifiedPerform(actionContext);
            }
        }
    }
}
