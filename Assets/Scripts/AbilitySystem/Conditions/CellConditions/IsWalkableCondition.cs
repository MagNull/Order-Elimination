using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class IsWalkableCondition : ICellCondition
    {
        public ICellCondition Clone()
        {
            var clone = new IsWalkableCondition();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
        {
            var entities = battleContext.BattleMap.GetContainedEntities(positionToCheck).ToArray();
            return entities.All(e => e.Obstacle.IsAllowedToWalk(askingEntity));
        }
    }
}
