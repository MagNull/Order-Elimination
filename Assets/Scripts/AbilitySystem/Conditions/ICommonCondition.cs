using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;

namespace OrderElimination.AbilitySystem
{
    [GUIColor(1, 1, 0)]
    public interface ICommonCondition : ICloneable<ICommonCondition>
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity);
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, CellGroupsContainer cellGroups);
	}
}
