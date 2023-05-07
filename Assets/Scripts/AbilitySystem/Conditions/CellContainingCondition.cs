using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CellContainingCondition : ICellCondition
    {
        [ShowInInspector, OdinSerialize]
        public bool MustBeEmpty { get; private set; }

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public EntityFilter EntityFilter { get; private set; } = new EntityFilter();

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool AllowEmptyCells { get; private set; }

        [HideIf("@" + nameof(MustBeEmpty) + " == true")]
        [ShowInInspector, OdinSerialize]
        public bool AllEntitiesMustMeetRequirements { get; private set; }

        public bool IsConditionMet(IBattleContext battleContext, AbilitySystemActor caster, Vector2Int cellPosition)
        {
            var cellEntities = battleContext.BattleMap.GetContainingEntities(cellPosition).ToArray();
            var cellIsEmpty = cellEntities.Length == 0;
            if (MustBeEmpty) return cellIsEmpty;
            if (AllEntitiesMustMeetRequirements)
            {
                if (cellIsEmpty)
                    return AllowEmptyCells;
                return cellEntities.All(e => EntityFilter.IsAllowed(battleContext, caster, e))
                    || AllowEmptyCells && cellIsEmpty;
            }
            else
            {
                if (cellIsEmpty)
                    return AllowEmptyCells;
                return cellEntities.Any(e => EntityFilter.IsAllowed(battleContext, caster, e));
            }
        }
    }
}
