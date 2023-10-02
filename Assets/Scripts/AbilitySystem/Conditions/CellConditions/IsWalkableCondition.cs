using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class IsWalkableCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public EntityFilter IgnoredObstacles { get; private set; } = new();

        public ICellCondition Clone()
        {
            var clone = new IsWalkableCondition();
            clone.IgnoredObstacles = IgnoredObstacles.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
            => IsConditionMet(battleContext, askingEntity, positionToCheck, null);

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck, CellGroupsContainer cellGroups)
        {
            var entities = battleContext.BattleMap.GetContainedEntities(positionToCheck).ToArray();
            return entities.All(e =>
            e.Obstacle.IsAllowedToWalk(askingEntity)
            || IgnoredObstacles.IsAllowed(battleContext, askingEntity, e));
        }
    }
}
