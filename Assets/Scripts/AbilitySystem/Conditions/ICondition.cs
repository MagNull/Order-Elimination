using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
	public interface ICommonCondition
	{
		public bool IsConditionMet(IBattleContext battleContext, IAbilitySystemActor caster);
	}

    public interface ICellCondition
    {
        public bool IsConditionMet(IBattleContext battleContext, IAbilitySystemActor caster, Vector2Int cellPosition);
    }

	public interface IActionCondition
	{
		public bool IsConditionMet(IBattleContext battleContext, IAbilitySystemActor caster, IAbilitySystemActor target);
	}

    public class DistanceToTargetCondition : ICellCondition
    {
		public float MinDistance { get; }
		public float MaxDistance { get; }

		public bool IsConditionMet(IBattleContext context, IAbilitySystemActor caster, Vector2Int targetCell)
		{
			var casterCell = context.BattleMap.GetPosition(caster);
			var distanceToTarget = context.BattleMap.GetDistanceBetween(casterCell, targetCell);

            return distanceToTarget <= MaxDistance && distanceToTarget >= MinDistance;
		}
    }
}
