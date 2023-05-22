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
                UpdateZone();
            }

            void OnDeactivation(ITriggerSetup.BattleTrigger trigger)
            {
                trigger.Deactivated -= OnDeactivation;
                trigger.OperatingContext.BattleMap.CellChanged -= OnCellChanged;
            }

            void UpdateZone()
            {
                var entityPos = trackingEntity.Position;
                var context = instance.OperatingContext;
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
                    instance.Trigger(new TriggerZoneFireInfo(currentEntities, newEntities, disappearedEntities));
                }
            }

            void OnCellChanged(Vector2Int cellPos)
            {
                UpdateZone();
            }
        }
    }

    public readonly struct TriggerZoneFireInfo : ITriggerFireInfo
    {
        public readonly AbilitySystemActor[] CurrentEntities;
        public readonly AbilitySystemActor[] NewEntities;
        public readonly AbilitySystemActor[] DisappearedEntities;

        public TriggerZoneFireInfo(
            AbilitySystemActor[] currentEntities, 
            AbilitySystemActor[] newEntities, 
            AbilitySystemActor[] disappearedEntities)
        {
            CurrentEntities = currentEntities;
            NewEntities = newEntities;
            DisappearedEntities = disappearedEntities;
        }
    }
}
