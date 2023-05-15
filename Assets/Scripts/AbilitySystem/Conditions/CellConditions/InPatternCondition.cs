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

        public bool IsConditionMet(IBattleContext context, AbilitySystemActor caster, Vector2Int targetPosition)
		{
			var casterPos = context.BattleMap.GetPosition(caster);
			var pattern = Pattern.GetAbsolutePositions(casterPos);

            return pattern.Contains(targetPosition);
		}
    }
}
