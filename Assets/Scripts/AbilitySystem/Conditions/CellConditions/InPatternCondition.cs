using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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
            => IsConditionMet(context, askingEntity, positionToCheck, null);

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor askingEntity, Vector2Int positionToCheck, CellGroupsContainer cellGroups)
        {
            var casterPos = askingEntity.Position;
            return Pattern.ContainsPositionWithOrigin(positionToCheck, casterPos);
        }

        Vector2Int[] ICellCondition.FilterMany(
            IBattleContext battleContext, AbilitySystemActor askingEntity, IEnumerable<Vector2Int> positions, CellGroupsContainer cellGroups)
        {
            var casterPos = askingEntity.Position;
            var pattern = Pattern.GetAbsolutePositions(casterPos).ToHashSet();
            return positions.Where(p => pattern.Contains(p)).ToArray();
        }
    }
}
