using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [GUIColor(1, 1, 0)]
    public interface ICommonCondition : ICloneable<ICommonCondition>
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster);
	}

	[GUIColor(1, 1, 0)]
    public interface ICellCondition : ICloneable<ICellCondition>
    {
        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition);
    }

    [GUIColor(1, 1, 0)]
    public interface IEntityCondition : ICloneable<IEntityCondition>
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, AbilitySystemActor entity);
	}
}
