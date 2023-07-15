using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class AllowsToStayCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public EntityFilter IgnoredObstacles { get; private set; } = new();

        public ICellCondition Clone()
        {
            var clone = new AllowsToStayCondition();
            clone.IgnoredObstacles = IgnoredObstacles.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
        {
            var entities = battleContext.BattleMap.GetContainedEntities(positionToCheck).ToArray();
            return entities.All(e => 
            e.Obstacle.IsAllowedToStay(askingEntity)
            || IgnoredObstacles.IsAllowed(battleContext, askingEntity, e));
        }
    }
}
