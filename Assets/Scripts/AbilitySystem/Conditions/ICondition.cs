using OrderElimination.Infrastructure;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface ICommonCondition : ICloneable<ICommonCondition>
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster);
	}

    public interface ICellCondition : ICloneable<ICellCondition>
    {
        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition);
    }

	public interface IEntityCondition : ICloneable<IEntityCondition>
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor entity);
	}
}
