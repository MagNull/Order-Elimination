using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class MultipleEntityTrigger : IContextTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public IEntityTriggerSetup EntityTriggerSetup { get; private set; }

        [ShowInInspector, OdinSerialize]
        public IEntityCondition[] TrackingEntitiesConditions { get; private set; } = new IEntityCondition[0];

        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor activator)
        {
            var entityTriggers = new Dictionary<AbilitySystemActor, IBattleTrigger>();
            var instance = new ITriggerSetup.BattleTrigger(this, battleContext, activator);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.ActivationRequested -= OnActivation;
                instance.DeactivationRequested += OnDeactivation;
                var entitiesBank = battleContext.EntitiesBank;
                entitiesBank.EntityAdded += OnEntityAdded;
                entitiesBank.EntityDisposed += OnEntityDisposed;
                foreach (var entity in entitiesBank.GetActiveEntities())
                {
                    HandleNewEntity(entity);
                }

            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                instance.DeactivationRequested -= OnDeactivation;
                battleContext.EntitiesBank.EntityAdded -= OnEntityAdded;
                battleContext.EntitiesBank.EntityDisposed -= OnEntityDisposed;
                foreach (var entity in entityTriggers.Keys.ToArray())
                    StopTrackingEntity(entity);
            }

            void OnEntityAdded(IReadOnlyEntitiesBank entitiesBank, AbilitySystemActor entity)
                => HandleNewEntity(entity);

            void OnEntityDisposed(IReadOnlyEntitiesBank entitiesBank, AbilitySystemActor entity)
            {
                if (entityTriggers.ContainsKey(entity))
                    StopTrackingEntity(entity);
            }

            void HandleNewEntity(AbilitySystemActor entity)
            {
                if (entityTriggers.ContainsKey(entity))
                    throw new InvalidOperationException("Entity is already tracking.");
                if (TrackingEntitiesConditions.AllMet(battleContext, activator, entity))
                {
                    var trigger = EntityTriggerSetup.GetTrigger(battleContext, activator, entity);
                    entityTriggers.Add(entity, trigger);
                    trigger.Triggered += OnTriggerFired;
                    trigger.Activate();
                }
            }

            void StopTrackingEntity(AbilitySystemActor entity)
            {
                var trigger = entityTriggers[entity];
                trigger.Triggered -= OnTriggerFired;
                trigger.Deactivate();
                entityTriggers.Remove(entity);
            }

            void OnTriggerFired(ITriggerFireInfo fireInfo) => instance.FireTrigger(fireInfo);
        }
    }
}
