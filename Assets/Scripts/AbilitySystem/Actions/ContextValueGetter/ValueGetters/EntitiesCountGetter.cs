using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public class EntitiesCountGetter : IContextValueGetter
    {
        [ShowInInspector, OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; } = new IEntityCondition[0];

        [ShowInInspector, OdinSerialize]
        public int CountInCellGroupId { get; private set; }

        public string DisplayedFormula => $"(EntitiesCount in {CountInCellGroupId})";

        public bool CanBePrecalculatedWith(ValueCalculationContext context)
        {
            return context.CellTargetGroups != null;
        }

        public IContextValueGetter Clone()
        {
            var clone = new EntitiesCountGetter();
            clone.EntityConditions = EntityConditions.DeepClone();
            clone.CountInCellGroupId = CountInCellGroupId;
            return clone;
        }

        public float GetValue(ValueCalculationContext context)
        {
            var cellGroups = context.CellTargetGroups;
            if (!cellGroups.ContainsGroup(CountInCellGroupId))
                return 0;
            var cellsToCheck = cellGroups.GetGroup(CountInCellGroupId);
            var entitiesCount = 0;
            var conditions = EntityConditions;
            var battleContext = context.BattleContext;
            var map = battleContext.BattleMap;
            foreach (var pos in cellsToCheck)
            {
                entitiesCount += map.GetContainedEntities(pos)
                    .Where(e => IsEntityAllowed(e)).Count();
            }
            return entitiesCount;

            bool IsEntityAllowed(AbilitySystemActor entity)
                => conditions.AllMet(battleContext, context.BattleCaster, entity);
        }
    }
}
