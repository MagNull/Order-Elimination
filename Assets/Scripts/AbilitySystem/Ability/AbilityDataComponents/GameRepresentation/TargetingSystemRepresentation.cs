using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class TargetingSystemRepresentation
    {
        public TargetingSystemType TargetingType { get; }

        public float MaxTargetingRange { get; } = float.PositiveInfinity;

        public TargetingSystemRepresentation(IAbilityTargetingSystem targetingSystem)
        {
            ICellGroupsDistributor cellDistributor;
            ICellCondition[] cellConditions = null;
            #region TargetingSystemImplementations
            if (targetingSystem is NoTargetTargetingSystem noTargetSystem)
            {
                TargetingType = TargetingSystemType.NoTarget;
                MaxTargetingRange = 0;
                cellDistributor = noTargetSystem.CellGroupsDistributor;

            }
            else if (targetingSystem is SingleTargetTargetingSystem singleTargetSystem)
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
            #endregion
            #region CellConditionImplementations
            if (cellConditions != null)
            {
                foreach (var condition in cellConditions)
                {
                    if (condition is AnyCellCondition anyCellCondition)
                    {

                    }
                    else if (condition is InPatternCondition patternCondition)
                    {

                    }
                    else if (condition is HasPathCondition pathCondition)
                    {

                    }
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
    }
}
