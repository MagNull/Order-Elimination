using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;

namespace OrderElimination.AbilitySystem
{
    [GUIColor(1, 1, 0)]
    public interface IEntityCondition : ICloneable<IEntityCondition>
	{
		public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, AbilitySystemActor entityToCheck);
	}
}
