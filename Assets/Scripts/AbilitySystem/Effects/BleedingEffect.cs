using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class BleedingEffect : IEffect, ITemporaryEffect, IPeriodicEffect
    {
        public bool IsActive { get; private set; }
        public bool IsStackable => true;
        public bool UseApplierActionProcessor => false;
        public bool CanBeForceRemoved => true;
        public int ApplyingDuration { get; set; } = 5;
        public int LeftDuration { get; private set; } = -1;
        public int PeriodLength => 1;
        public IEnumerable<IBattleAction> ActionsOnApply => Enumerable.Empty<IBattleAction>();
        public IEnumerable<IBattleAction> ActionsOnRemove => Enumerable.Empty<IBattleAction>();

        public IAbilitySystemActor EffectApplier { get; private set; }
        public IAbilitySystemActor EffectHolder { get; private set; }

        public IEnumerable<IBattleAction> ActionsPerPeriod => _actionsPerPeriod;

        public event Action<IEffect> Deactivated;
        public event Action<ITemporaryEffect> DurationEnded;

        private List<IBattleAction> _actionsPerPeriod;

        public bool Activate(IAbilitySystemActor effectTarget, IAbilitySystemActor effectApplier)
        {
            if (IsActive)
                return false;
            EffectApplier = effectApplier;
            EffectHolder = effectTarget;
            LeftDuration = ApplyingDuration;
            IsActive = true;
            return true;
        }

        public bool Deactivate()
        {
            if (!IsActive)
                return false;
            //Do Actions on remove
            EffectApplier = null;
            EffectHolder = null;
            LeftDuration = -1;
            IsActive = false;
            return true;
        }

        public void OnNewRoundCallback(IBattleContext battleContext)
        {
            if (!IsActive) return;
            //Do stuff
            if ((ApplyingDuration - LeftDuration) % PeriodLength == 0)
            {
                var actionContext = new ActionExecutionContext(battleContext, EffectApplier, EffectHolder);
                foreach (var action in _actionsPerPeriod)
                {
                    action.ModifiedPerform(actionContext, UseApplierActionProcessor, true);
                }
            }
            LeftDuration--;
            if (LeftDuration <= 0)
            {
                DurationEnded?.Invoke(this);
            }
        }
    }
}
