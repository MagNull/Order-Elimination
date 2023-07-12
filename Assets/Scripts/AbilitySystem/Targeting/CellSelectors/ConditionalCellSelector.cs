using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ConditionalCellSelector : ICellSelector
    {
        [ShowInInspector, OdinSerialize]
        public ICellCondition[] CellConditions { get; private set; } = new ICellCondition[0];

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var battleContext = context.BattleContext;
            var askingEntity = context.AskingEntity;
            return context.PositionsPool
                .Where(p => context.BattleContext.BattleMap.ContainsPosition(p))
                .Where(p => CellConditions.All(c => c.IsConditionMet(battleContext, askingEntity, p)))
                .ToArray();
        }
    }
}
