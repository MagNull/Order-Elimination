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

    public interface IEffectData
    {
        //EffectView?
        public bool IsStackable { get; }
        public bool UseApplierActionProcessor { get; } //Использовать обработчик, наложившего эффект?
        public bool CanBeForceRemoved { get; }
        public IBattleAction[] ActionsOnApply { get; }
        public IBattleAction[] ActionsOnRemove { get; }
        //RemovedByTriggers
    }

    public interface ITemporaryEffectData : IEffectData
    {
        public int ApplyingDuration { get; }
    }

    public interface IEffect
    {
        public IEffectData EffectData { get; }
        public IAbilitySystemActor EffectApplier { get; } 
        public IAbilitySystemActor EffectHolder { get; }
        public event Action<IEffect> Finished;//Destroyed Removed Disposed Finished
        public bool Activate(IAbilitySystemActor effectTarget);
        public bool Deactivate();
    }

    public interface ITemporaryEffect : IEffect
    {
        public ITemporaryEffectData TemporaryEffectData { get; }
        public int LeftDuration { get; }
        public event Action<ITemporaryEffect> DurationEnded;
    }

    public interface IIncomingActionProcessingEffect<TAction> : IEffect where TAction : BattleAction<TAction>
    {
        public TAction ProcessIncomingAction(TAction originalAction);
    }

    public interface IOutcomingActionProcessingEffect<TAction> : IEffect where TAction : BattleAction<TAction>
    {
        public TAction ProcessOutcomingAction(TAction originalAction);
    }

    public interface IPeriodicEffect : IEffect
    {
        public int PeriodLength { get; } // 1Tick = 1 ход
        //Действия обязаны выполняться каждый промежуток ходов, пока активен эффект.
        //Действия должны прекратить выполняться при удалении эффекта.
        public IBattleAction[] ActionsPerPeriod { get; }
        public void OnTick(IBattleContext battleContext)
        {
            foreach (var action in ActionsPerPeriod)
            {
                var actionMaker = EffectData.UseApplierActionProcessor ? EffectApplier : null;
                var actionContext = new ActionExecutionContext(battleContext, actionMaker, EffectHolder);
                action.ModifiedPerform(actionContext);
            }
        }
    }

    public interface IConditionalEffect : IEffect // == PeriodicEffect ??
    {
        //Triggers
        //Actions
    }
}
