using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public interface IUtilizeCellGroupsAction : IBattleAction
    {
        public IEnumerable<int> UtilizingCellGroups { get; }

        public int GetUtilizedCellsAmount(int group);
    }
}
