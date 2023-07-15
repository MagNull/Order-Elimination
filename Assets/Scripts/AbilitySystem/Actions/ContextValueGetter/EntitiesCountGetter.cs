using OrderElimination.Infrastructure;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    [Serializable]
    public struct EntitiesCountGetter : IContextValueGetter
    {
        [OdinSerialize]
        public IEntityCondition[] EntityConditions { get; private set; }

        [OdinSerialize]
        public IPointRelativePattern PatternToCheck { get; private set; }

        [OdinSerialize]
        public ActionEntity RelativeTo { get; private set; }

        public string DisplayedFormula => "EntitiesCount";

        public IContextValueGetter Clone()
        {
            var clone = new EntitiesCountGetter();
            clone.EntityConditions = CloneableCollectionsExtensions.Clone(EntityConditions);
            clone.PatternToCheck = PatternToCheck.Clone();
            clone.RelativeTo = RelativeTo;
            return clone;
        }

        public float GetValue(ActionContext useContext)
        {
            var entitiesCount = 0;
            var conditions = EntityConditions;
            var battleContext = useContext.BattleContext;
            var map = battleContext.BattleMap;
            var target = RelativeTo switch
            {
                ActionEntity.Caster => useContext.ActionMaker,
                ActionEntity.Target => useContext.ActionTarget,
                _ => throw new NotImplementedException(),
            };
            var points = PatternToCheck.GetAbsolutePositions(target.Position);
            foreach (var pos in points.Where(p => map.CellRangeBorders.Contains(p)))
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
