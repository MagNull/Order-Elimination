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
            => $"{Left.DisplayedFormula} {ComparisonOperation.AsString()} {Right.DisplayedFormula}";

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
        {
            var calculationContext = new ValueCalculationContext(
                battleContext,
                CellGroupsContainer.Empty,
                askingEntity,
                entityToCheck);
            var left = Left.GetValue(calculationContext);
            var right = Right.GetValue(calculationContext);
            return ComparisonOperation switch
            {
                BinaryComparisonOperation.Equals => left == right,
                BinaryComparisonOperation.NotEquals => left != right,
                BinaryComparisonOperation.GreaterThan => left > right,
                BinaryComparisonOperation.LessThan => left < right,
                BinaryComparisonOperation.GreaterOrEquals => left >= right,
                BinaryComparisonOperation.LessOrEquals => left <= right,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
