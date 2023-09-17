using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public interface IUtilizeCellGroupsAction : IBattleAction
    {
        public int[] UtilizedCellGroups { get; }
    }
}
