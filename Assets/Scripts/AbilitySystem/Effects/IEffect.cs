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
        public bool IsActive { get; } //Instance
        public AbilitySystemActor EffectApplier { get; } //<Instance>
        public AbilitySystemActor EffectHolder { get; } //<Instance>

        public event Action<IEffect> Deactivated;//Destroyed Removed Disposed Finished
        public bool Activate(AbilitySystemActor effectTarget, AbilitySystemActor effectApplier);
        public bool Deactivate();

        public EffectView View { get; } //Data
        public bool IsStackable { get; } //Data
        public bool UseApplierProcessing{ get; } //Data
        public bool UseHolderProcessing { get; } //Data
        public bool CanBeForceRemoved { get; } //Data
        //Actions with applier and holder
        public IEnumerable<IBattleAction> InstructionsOnActivation { get; } //Data
        public IEnumerable<IBattleAction> InstructionsOnDeactivation { get; } //Data
        //RemovedByTriggers
    }

    public interface ITemporaryEffect : IEffect
    {
        public int ApplyingDuration { get; } //Data
        public int LeftDuration { get; } //Instance
        public event Action<ITemporaryEffect> DurationEnded; //Instance

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
        public int PeriodLength { get; } //Data
        public bool PerformOnApply { get; } //Data
        //Действия обязаны выполняться каждый промежуток ходов, пока активен эффект.
        //Действия должны прекратить выполняться при удалении эффекта.
        public IEnumerable<IBattleAction> ActionsPerPeriod { get; }
        public void OnNewRoundCallback(IBattleContext battleContext)
        {
            var automatedCellGroups = new CellGroupsContainer();

            foreach (var action in ActionsPerPeriod)
            {
                var actionMaker = UseApplierProcessing ? EffectApplier : null;
                var actionContext = new ActionContext(battleContext, automatedCellGroups, actionMaker, EffectHolder);
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
