using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct EntitiesCountGetter : IContextValueGetter
    {
        [ShowInInspector, OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int CountInCellGroupId { get; private set; }

        public string DisplayedFormula => "EntitiesCount";

        public IContextValueGetter Clone()
        {
            var clone = new EntitiesCountGetter();
            clone.EntityConditions = CloneableCollectionsExtensions.DeepClone(EntityConditions);
            clone.CountInCellGroupId = CountInCellGroupId;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            var cellGroups = useContext.TargetCellGroups;
            if (!cellGroups.ContainsGroup(CountInCellGroupId))
                return 0;
            var cellsToCheck = cellGroups.GetGroup(CountInCellGroupId);
            var entitiesCount = 0;
            var conditions = EntityConditions;
            var battleContext = useContext.BattleContext;
            var map = battleContext.BattleMap;
            foreach (var pos in cellsToCheck)
            {
                entitiesCount += map.GetContainedEntities(pos)
                    .Where(e => IsEntityAllowed(e)).Count();
            }
            return entitiesCount;

            bool IsEntityAllowed(AbilitySystemActor entity)
                => conditions.All(c => c.IsConditionMet(battleContext, useContext.ActionMaker, entity));
        }
    }
}
