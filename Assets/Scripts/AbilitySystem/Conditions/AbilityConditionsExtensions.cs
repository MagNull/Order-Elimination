using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public static class AbilityConditionsExtensions
    {
        public static bool AllMet(this IEnumerable<ICommonCondition> conditions, 
            IBattleContext battleContext, 
            AbilitySystemActor askingEntity)
        {
            return conditions.All(c => c.IsConditionMet(battleContext, askingEntity));
        }

        public static bool AllMet(this IEnumerable<ICommonCondition> conditions,
            IBattleContext battleContext,
            AbilitySystemActor askingEntity,
            CellGroupsContainer cellGroups)
        {
            return conditions.All(c => c.IsConditionMet(battleContext, askingEntity, cellGroups));
        }

        public static bool AllMet(this IEnumerable<ICellCondition> conditions,
            IBattleContext battleContext,
            AbilitySystemActor askingEntity,
            Vector2Int cellPosition)
        {
            return conditions.All(c => c.IsConditionMet(battleContext, askingEntity, cellPosition));
        }

        public static bool AllMet(this IEnumerable<ICellCondition> conditions,
            IBattleContext battleContext,
            AbilitySystemActor askingEntity,
            Vector2Int cellPosition,
            CellGroupsContainer cellGroups)
        {
            return conditions.All(c => c.IsConditionMet(battleContext, askingEntity, cellPosition, cellGroups));
        }

        //Pass CellGroups after adding method with them to CellConditions
        public static bool AllMet(this IEnumerable<IEntityCondition> conditions,
            IBattleContext battleContext,
            AbilitySystemActor askingEntity,
            AbilitySystemActor entityToCheck)
        {
            return conditions.All(c => c.IsConditionMet(battleContext, askingEntity, entityToCheck));
        }

        public static bool AllMet(this IEnumerable<IEntityCondition> conditions,
            IBattleContext battleContext,
            AbilitySystemActor askingEntity,
            AbilitySystemActor entityToCheck,
            CellGroupsContainer cellGroups)
        {
            return conditions.All(c => c.IsConditionMet(battleContext, askingEntity, entityToCheck, cellGroups));
        }
    }
}
