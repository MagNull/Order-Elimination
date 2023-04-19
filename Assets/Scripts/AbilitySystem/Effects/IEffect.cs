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

    public enum EffectStackingPolicy
    {
        DontApply,
        Override,
        Stack
    }

    public interface IEffect
    {
        //static EffectView : ScriptableObject - name, icon, descr ?
        public bool IsActive { get; }
        public bool IsStackable { get; }
        public bool UseApplierActionProcessor { get; } //Использовать обработчик, наложившего эффект?
        public bool CanBeForceRemoved { get; }
        public IEnumerable<IBattleAction> ActionsOnApply { get; }
        public IEnumerable<IBattleAction> ActionsOnRemove { get; }

        public IAbilitySystemActor EffectApplier { get; } 
        public IAbilitySystemActor EffectHolder { get; }
        public event Action<IEffect> Deactivated;//Destroyed Removed Disposed Finished
        public bool Activate(IAbilitySystemActor effectTarget, IAbilitySystemActor effectApplier);
        public bool Deactivate();
        //RemovedByTriggers
    }

    public interface ITemporaryEffect : IEffect
    {
        public int ApplyingDuration { get; }
        public int LeftDuration { get; }
        public event Action<ITemporaryEffect> DurationEnded;

        public void OnNewRoundCallback(IBattleContext battleContext);
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
        public IEnumerable<IBattleAction> ActionsPerPeriod { get; }
        public void OnNewRoundCallback(IBattleContext battleContext)
        {
            foreach (var action in ActionsPerPeriod)
            {
                var actionMaker = UseApplierActionProcessor ? EffectApplier : null;
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
