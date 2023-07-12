using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ICellGroupsDistributor
    {
        public CellGroupsContainer DistributeSelection(
            IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int[] selectedPositions);
    }
}
