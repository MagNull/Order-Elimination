using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
namespace OrderElimination.AbilitySystem
{
    public class EntityUsedAbilityTrigger : IEntityTriggerSetup
    {
        private enum TriggerAfterOption
        {
            AbilityStarted,
            AbilityCompleted
        }

        [ShowInInspector, OdinSerialize]
        private TriggerAfterOption TriggerAfter { get; set; }

        [ShowInInspector, OdinSerialize]
        private List<ActiveAbilityBuilder> _ignoredAbilities = new();

        public IBattleTrigger GetTrigger(
            IBattleContext battleContext, AbilitySystemActor activator, AbilitySystemActor trackingEntity)
        {
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext, activator);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                foreach (var runner in trackingEntity.ActiveAbilities)
                {
                    switch (TriggerAfter)
                    {
                        case TriggerAfterOption.AbilityStarted:
                            runner.AbilityExecutionStarted += OnAbilityUsed;
                            break;
                        case TriggerAfterOption.AbilityCompleted:
                            runner.AbilityExecutionCompleted += OnAbilityUsed;
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                foreach (var runner in trackingEntity.ActiveAbilities)
                {
                    runner.AbilityExecutionStarted -= OnAbilityUsed;
                    runner.AbilityExecutionCompleted -= OnAbilityUsed;
                }
            }

            void OnAbilityUsed(ActiveAbilityRunner runner)
            {
                if (!_ignoredAbilities.Contains(runner.AbilityData.BasedBuilder))
                    instance.FireTrigger(new EntityUsedAbilityFireInfo(instance, runner.AbilityData));
            }
        }
    }
}
