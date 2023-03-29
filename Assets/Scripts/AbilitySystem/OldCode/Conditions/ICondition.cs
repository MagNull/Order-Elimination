using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
	//����� ������� ����������� (����� ��������� 1 ���):
    //���� ����� Conditions(IBattleContext) (�: ����� ���)
    //���� CasterConditions(Caster) (�: ������� �������)
	//������� ����������� ������ (��������� ��� ������ ������):
    //���� CellConditions(IBattleContext, Cell) (�: ������ ������, � ���� ������ 50% ��)
    //���� ��������������� Conditions(IBattleContext, Caster, Cell) (�: )

    public interface ICondition { }

	//TODO: �������� useContext � caster, casterCell, targetcell, ...
	//������ �������� ��� ������, ���� �������� ���� �� ���� �������� � ���?
	
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
