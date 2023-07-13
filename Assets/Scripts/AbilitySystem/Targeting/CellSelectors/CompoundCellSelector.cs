using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CompoundCellSelector : ICellSelector
    {
        [ShowInInspector, OdinSerialize]
        public ICellSelector SelectorA { get; private set; }

        [ShowInInspector, OdinSerialize]
        public BooleanOperation BooleanOperation { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ICellSelector SelectorB { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            //Optimization
            var selectionA = SelectorA.GetCellPositions(context)
                .Where(p => context.BattleContext.BattleMap.ContainsPosition(p))
                .ToArray();
            Vector2Int[] selectionB;
            if (BooleanOperation == BooleanOperation.Intersect
                || BooleanOperation == BooleanOperation.Except)
            {
                var truncatedContext = new CellSelectorContext(
                    context.BattleContext,
                    selectionA,
                    context.AskingEntity,
                    context.SelectedCellPositions);
                selectionB = SelectorB.GetCellPositions(truncatedContext);
            }
            else
                selectionB = SelectorB.GetCellPositions(context);
            //Optimization

            var result = BooleanOperation switch
            {
                BooleanOperation.Union => selectionA.Union(selectionB),
                BooleanOperation.Intersect => selectionA.Intersect(selectionB),
                BooleanOperation.Except => selectionA.Except(selectionB),
                _ => throw new System.NotImplementedException(),
            };
            return result.Where(p => context.BattleContext.BattleMap.ContainsPosition(p)).ToArray();
        }
    }
}
