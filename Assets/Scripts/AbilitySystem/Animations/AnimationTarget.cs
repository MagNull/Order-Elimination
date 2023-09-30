using OrderElimination.Infrastructure;
using UnityEngine;

namespace OrderElimination.AbilitySystem.Animations
{
    public enum AnimationTarget
    {
        Target,
        Caster,
        CellGroup,
        //WorldPosition
    }

    public static class AnimationTargetHelpers
    {
        public static Vector2Int SelectGamePosition(
            this AnimationTarget target,
            Vector2Int? casterPos,
            Vector2Int? targetPos,
            CellGroupsContainer cellGroups,
            int cellGroup,
            CellPriority groupPriority = CellPriority.FirstInGroup)
        {
            return target switch
            {
                AnimationTarget.Target => targetPos.Value,
                AnimationTarget.Caster => casterPos.Value,
                AnimationTarget.CellGroup => groupPriority.GetPositionByPriority(
                    cellGroups.GetGroup(cellGroup), casterPos, targetPos),
                _ => throw new System.NotImplementedException(),
            };
        }
    }
}
