using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
	public interface ICommonCondition
	{
		public bool IsConditionMet(IBattleContext context, IAbilitySystemActor caster);
	}

    public interface ICellCondition
    {
        public bool IsConditionMet(IBattleContext context, IAbilitySystemActor caster, Cell targetCell);
    }

	public interface IActionCondition
	{
		public bool IsConditionMet(IBattleContext battleContext, IAbilitySystemActor caster, IAbilitySystemActor target);
	}

    public class DistanceToTargetCondition : ICellCondition
    {
		public float MinDistance { get; }
		public float MaxDistance { get; }

		public bool IsConditionMet(IBattleContext context, IAbilitySystemActor caster, Cell targetCell)
		{
			var casterCell = context.BattleMap.GetCell(caster);
			var distanceToTarget = context.BattleMap.GetDistanceBetween(casterCell, targetCell);

            return distanceToTarget <= MaxDistance && distanceToTarget >= MinDistance;
		}
    }
}
