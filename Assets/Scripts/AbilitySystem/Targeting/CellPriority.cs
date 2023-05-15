using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public enum CellPriority
    {
        FirstInGroup,
        LastInGroup,
        ClosestToCaster,
        FurthestFromCaster,
        ClosestToTarget,
        FurthestFromTarget
    }
}
