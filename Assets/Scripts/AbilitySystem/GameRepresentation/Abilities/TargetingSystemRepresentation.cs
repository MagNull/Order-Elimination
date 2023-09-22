using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TargetingSystemRepresentation
    {
        public TargetingSystemType TargetingType { get; }

        public IPointRelativePattern TargetingPattern { get; private set; }

        public float MaxMathRange { get; private set; } = float.PositiveInfinity;
        public int MaxSquareRange { get; private set; } = int.MaxValue;

        public TargetingSystemRepresentation(IAbilityTargetingSystem targetingSystem)
        {
            ICellGroupsDistributor cellDistributor;
            ICellCondition[] cellConditions = null;

            #region TargetingSystemImplementations
            if (targetingSystem is NoTargetTargetingSystem noTargetSystem)
            {
                TargetingType = TargetingSystemType.NoTarget;
                MaxMathRange = 0;
                MaxSquareRange = 0;
                cellDistributor = noTargetSystem.CellGroupsDistributor;

            }
            else if (targetingSystem is IRequireSelectionTargetingSystem manualTargeting)
            {
                if (targetingSystem is SingleTargetTargetingSystem singleTargetSystem)
                {
                    TargetingType = TargetingSystemType.SingleTarget;
                    cellDistributor = singleTargetSystem.CellGroupsDistributor;
                    cellConditions = singleTargetSystem.CellConditions.ToArray();

                }
                else if (targetingSystem is MultiTargetTargetingSystem multiTargetSystem)
                {
                    TargetingType = TargetingSystemType.MultiTarget;
                    cellDistributor = multiTargetSystem.CellGroupsDistributor;
                    cellConditions = multiTargetSystem.CellConditions.ToArray();
                }
                else
                    throw new NotImplementedException();
            }
            else
                throw new NotImplementedException();
            #endregion

            #region CellConditionImplementations
            if (cellConditions != null)
            {
                foreach (var condition in cellConditions)
                {
                    DescribeCondition(condition);
                }
            }
            #endregion

            #region DistributorImplementations
            if (cellDistributor is BasicCellGroupsDistributor basicDistributor)
            {
                var selectors = basicDistributor.GroupSelectors;
                //...
            }
            else
                throw new NotImplementedException();
            #endregion
        }

        private void DescribeCondition(ICellCondition condition, bool anyCondition = false)
        {
            if (condition is AnyCellCondition anyCellCondition)
            {
                foreach (var subCondition in anyCellCondition.CellConditions)
                    DescribeCondition(subCondition, true);
            }
            else if (condition is NegateCellCondition negateCellCondition)
            {
                Logging.LogException(new NotImplementedException(
                    $"Can not describe {condition.GetType().Name} for Game Representation."));
            }
            else if (condition is InPatternCondition patternCondition)
            {
                if (TargetingPattern == null)
                {
                    TargetingPattern = patternCondition.Pattern;
                }
                else
                {
                    TargetingPattern = new CompoundPointPattern()
                    {
                        PatternA = TargetingPattern,
                        PatternB = patternCondition.Pattern,
                        BooleanOperation = anyCondition ? BooleanOperation.Union : BooleanOperation.Intersect
                    };
                }
                MaxMathRange = TargetingPattern.GetMaxDistance(Vector2Int.zero);
                MaxSquareRange = TargetingPattern.GetMaxSquareDistance(Vector2Int.zero);
            }
            else if (condition is HasPathCondition pathCondition)
            {
                if (pathCondition.LimitByCasterMovement)
                {
                    //No any info about caster in AbilityData
                }
            }
        }
    }
}
