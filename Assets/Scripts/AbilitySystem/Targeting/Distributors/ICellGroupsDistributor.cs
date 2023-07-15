using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ICellGroupsDistributor
    {
        public CellGroupsContainer DistributeSelection(
            IBattleContext battleContext, 
            AbilitySystemActor askingEntity, 
            IEnumerable<Vector2Int> selectedPositions);
    }
}
