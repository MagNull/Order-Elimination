using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace OrderElimination.AbilitySystem
{
    public class EntityValueCheckCondition : IEntityCondition
    {
        [ShowInInspector, DisplayAsString]
        private string _comparisonFormula
            => $"{Left?.DisplayedFormula ?? "???"} {ComparisonOperation.AsString()} {Right?.DisplayedFormula ?? "???"}";

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter Left { get; private set; } = new ConstValueGetter(0);

        [ShowInInspector, OdinSerialize]
        public BinaryComparisonOperation ComparisonOperation { get; private set; }

        [ShowInInspector, OdinSerialize]
        public IContextValueGetter Right { get; private set; } = new ConstValueGetter(0);

        public IEntityCondition Clone()
        {
            var clone = new EntityValueCheckCondition();
            clone.Left = Left.Clone();
            clone.ComparisonOperation = ComparisonOperation;
            clone.Right = Right.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck)
            => IsConditionMet(battleContext, askingEntity, entityToCheck, null);

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck, CellGroupsContainer cellGroups)
        {
            var calculationContext = ValueCalculationContext.Full(
                battleContext,
                cellGroups,
                askingEntity,
                entityToCheck);
            var left = Left.GetValue(calculationContext);
            var right = Right.GetValue(calculationContext);
            return MathExtensions.CompareValues(left, right, ComparisonOperation);
        }
    }
}
