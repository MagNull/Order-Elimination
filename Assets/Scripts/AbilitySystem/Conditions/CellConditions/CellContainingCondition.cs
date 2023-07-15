using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellContainingCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public bool MustBeEmpty { get; private set; }

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public EntityFilter EntityFilter { get; private set; } = new EntityFilter();

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool VisibleEntitiesOnly { get; private set; } = true;

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool AllowEmptyCells { get; private set; }

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool AllEntitiesMustMeetRequirements { get; private set; }

        public ICellCondition Clone()
        {
            var clone = new CellContainingCondition();
            clone.MustBeEmpty = MustBeEmpty;
            clone.EntityFilter = EntityFilter;
            clone.VisibleEntitiesOnly = VisibleEntitiesOnly;
            clone.AllowEmptyCells = AllowEmptyCells;
            clone.AllEntitiesMustMeetRequirements = AllEntitiesMustMeetRequirements;
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int ositionToCheck)
        {
            AbilitySystemActor[] cellEntities;
            if (VisibleEntitiesOnly)
                cellEntities = battleContext.GetVisibleEntitiesAt(ositionToCheck, askingEntity.BattleSide).ToArray();
            else
                cellEntities = battleContext.BattleMap.GetContainedEntities(ositionToCheck).ToArray();
            var cellIsEmpty = cellEntities.Length == 0;
            if (MustBeEmpty) return cellIsEmpty;
            if (AllEntitiesMustMeetRequirements)
            {
                if (cellIsEmpty)
                    return AllowEmptyCells;
                return cellEntities.All(e => EntityFilter.IsAllowed(battleContext, askingEntity, e))
                    || AllowEmptyCells && cellIsEmpty;
            }
            else
            {
                if (cellIsEmpty)
                    return AllowEmptyCells;
                return cellEntities.Any(e => EntityFilter.IsAllowed(battleContext, askingEntity, e));
            }
        }
    }
}
