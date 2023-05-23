using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class EntityRelativeZoneTrigger : IEntityTriggerSetup
    {
        [ShowInInspector, OdinSerialize]
        public IPointRelativePattern ZonePattern { get; private set; }

        [ShowInInspector, OdinSerialize]
        public EntityFilter TriggeringEntities { get; private set; } = new();

        [ShowInInspector, OdinSerialize]
        public bool TriggerOnEnter { get; private set; } = true;
        
        [ShowInInspector, OdinSerialize]
        public bool TriggerOnExit { get; private set; }

        public IBattleTrigger GetTrigger(IBattleContext battleContext, AbilitySystemActor trackingEntity)
        {
            var entitiesInZone = new HashSet<AbilitySystemActor>();

            var instance = new ITriggerSetup.BattleTrigger(this, battleContext);
            instance.Activated += OnActivation;
            return instance;

            void OnActivation(ITriggerSetup.BattleTrigger trigger)
            {
                trigger.Activated -= OnActivation;
                trigger.Deactivated += OnDeactivation;
                trigger.OperatingContext.BattleMap.CellChanged += OnCellChanged;
                UpdateZone(instance, trackingEntity, ref entitiesInZone);
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                if (trigger != instance) throw new ArgumentException();
                trigger.Deactivated -= OnDeactivation;
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
            var entityPos = trackingEntity.Position;
            var context = triggerInstance.OperatingContext;
            var map = context.BattleMap;
            var zonePositions = ZonePattern
                .GetAbsolutePositions(entityPos)
                .Where(p => map.CellRangeBorders.Contains(p));

            var currentEntities = zonePositions
                .SelectMany(pos => map.GetContainedEntities(pos))
                .Where(entity => TriggeringEntities.IsAllowed(context, trackingEntity, entity))
                .ToArray();
            var disappearedEntities = entitiesInZone.Except(currentEntities).ToArray();
            var newEntities = currentEntities.Except(entitiesInZone).ToArray();
            if (disappearedEntities.Length == 0 && newEntities.Length == 0)
                return;

            entitiesInZone = currentEntities.ToHashSet();
            if (disappearedEntities.Length > 0 && TriggerOnExit
                || newEntities.Length > 0 && TriggerOnEnter)
            {
                Debug.Log("Trigger" % Colorize.Gold);
                triggerInstance.Trigger(new TriggerZoneFireInfo(
                    triggerInstance, currentEntities, newEntities, disappearedEntities));
            }
        }
    }

    public class TriggerZoneFireInfo : ITriggerFireInfo
    {
        public IBattleTrigger Trigger { get; }
        public AbilitySystemActor[] CurrentEntities { get; }
        public AbilitySystemActor[] NewEntities { get; }
        public AbilitySystemActor[] DisappearedEntities { get; }

        public TriggerZoneFireInfo(
            IBattleTrigger trigger,
            AbilitySystemActor[] currentEntities, 
            AbilitySystemActor[] newEntities, 
            AbilitySystemActor[] disappearedEntities)
        {
            Trigger = trigger;
            CurrentEntities = currentEntities;
            NewEntities = newEntities;
            DisappearedEntities = disappearedEntities;
        }
    }
}
