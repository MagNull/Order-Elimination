using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //Does not fire that entities left zone on deactivation!!
    public class EntityRelativeZoneTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public IPointRelativePattern ZonePattern { get; private set; }

        [ShowInInspector, OdinSerialize]
        public List<IEntityCondition> TriggeringEntityConditions { get; private set; } = new();
        //public EntityFilter TriggeringEntities { get; private set; } = new();

        [ShowInInspector, OdinSerialize]
        public bool TriggerOnEnter { get; private set; } = true;
        
        [ShowInInspector, OdinSerialize]
        public bool TriggerOnExit { get; private set; }

        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var entitiesInZone = new HashSet<AbilitySystemActor>();

            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.ActivationRequested += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                trigger.ActivationRequested -= OnActivation;
                trigger.DeactivationRequested += OnDeactivation;
                trigger.OperatingContext.BattleMap.CellChanged += OnCellChanged;
                UpdateZone(instance, trackingEntity, ref entitiesInZone);
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                if (trigger != instance) Logging.LogException( new ArgumentException());
                trigger.DeactivationRequested -= OnDeactivation;
                trigger.OperatingContext.BattleMap.CellChanged -= OnCellChanged;
            }

            void OnCellChanged(Vector2Int cellPos)//Fires twice on entity move
            {
                UpdateZone(instance, trackingEntity, ref entitiesInZone);
            }
        }

        private void UpdateZone(
                ITriggerSetup.BattleTrigger triggerInstance,
                AbilitySystemActor trackingEntity,
                ref HashSet<AbilitySystemActor> entitiesInZone)
        {
            var currentEntities = new AbilitySystemActor[0];
            if (!trackingEntity.IsDisposedFromBattle)
            {
                var entityPos = trackingEntity.Position;
                var context = triggerInstance.OperatingContext;
                var map = context.BattleMap;
                var zonePositions = ZonePattern
                    .GetAbsolutePositions(entityPos)
                    .Where(p => map.CellRangeBorders.Contains(p));

                currentEntities = zonePositions
                    .SelectMany(pos => map.GetContainedEntities(pos))
                    .Where(entity => TriggeringEntityConditions.All(c => c.IsConditionMet(context, trackingEntity, entity)))
                    .ToArray();
            }
            var disappearedEntities = entitiesInZone.Except(currentEntities).ToArray();
            var newEntities = currentEntities.Except(entitiesInZone).ToArray();
            if (disappearedEntities.Length == 0 && newEntities.Length == 0)
                return;

            entitiesInZone = currentEntities.ToHashSet();
            if (disappearedEntities.Length > 0 && TriggerOnExit
                || newEntities.Length > 0 && TriggerOnEnter)
            {
                //disappearedEntities.ForEach(e =>
                //{
                //    var view = e.BattleContext.EntitiesBank.GetViewByEntity(e);
                //    Logging.Log($"Entity {view.name} left zone" , Colorize.Green);
                //});
                //newEntities.ForEach(e =>
                //{
                //    var view = e.BattleContext.EntitiesBank.GetViewByEntity(e);
                //    Logging.Log($"Entity {view.name} entered zone" , Colorize.Green);
                //});
                Logging.Log($"Entities in zone [{currentEntities.Length}]" , Colorize.Green);
                triggerInstance.FireTrigger(new ZoneTriggerFireInfo(
                    triggerInstance, currentEntities, newEntities, disappearedEntities));
            }
        }
    }
}
