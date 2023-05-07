using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ICommonCondition
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster);
	}

    public interface ICellCondition
    {
        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition);
    }

	public interface ITargetCondition
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor target);
	}
}
