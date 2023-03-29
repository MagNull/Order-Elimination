using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
	//Общие условия доступности (нужно проверить 1 раз):
    //Есть общие Conditions(IBattleContext) (н: номер ход)
    //Есть CasterConditions(Caster) (н: хватает энергии)
	//Условия доступности клеток (проверять для каждой клетки):
    //Есть CellConditions(IBattleContext, Cell) (н: клетка пустая, у цели меньше 50% оз)
    //Есть комбинированные Conditions(IBattleContext, Caster, Cell) (н: )

    public interface ICondition { }

	//TODO: Добавить useContext с caster, casterCell, targetcell, ...
	//Клетка доступна для выбора, если доступна хотя бы одна сущность в ней?
	
	public interface ICommonCondition : ICondition
	{
		public bool IsConditionMet(IBattleContext context, IBattleEntity caster);
	}

    public interface ICellCondition : ICondition
    {
        public bool IsConditionMet(IBattleContext context, IAbilitySystemActor caster, Cell targetCell);
    }

	public interface ITargetCondition
	{
		public bool IsConditionMet(IBattleContext battleContext, IBattleEntity caster, IBattleEntity target);
	}

    public class DistanceToTargetCondition : ICellCondition
    {
		public float MaxDistance { get; }
		//ICondition -> ICellTargetCondition
		//IConditionContext
		//IAbilityUseContext
		public bool IsConditionMet(IBattleContext context, IBattleEntity caster, Cell target)
		{
			return context.GetDistanceBetween(caster, target) <= MaxDistance;
		}
    }

    public class DistanceToTargetCondition : ITargetCondition
    {
        public float MaxDistance { get; }
        //ICondition -> ICellTargetCondition
        //IConditionContext
        //IAbilityUseContext
        public bool IsConditionMet(IBattleContext context, IBattleEntity caster, Cell target)
        {
            return context.GetDistanceBetween(caster, target) <= MaxDistance;
        }
    }
}
