using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class InPatternCondition : ICellCondition
    {
		[ShowInInspector, SerializeField]
		public IPointRelativePattern Pattern { get; private set; }

        public ICellCondition Clone()
        {
            var clone = new InPatternCondition();
            clone.Pattern = Pattern.Clone();
            return clone;
        }

        public bool IsConditionMet(IBattleContext context, AbilitySystemActor askingEntity, Vector2Int positionToCheck)
		{
			var casterPos = askingEntity.Position;
			var pattern = Pattern.GetAbsolutePositions(casterPos);

            return pattern.Contains(positionToCheck);
		}
    }
}
