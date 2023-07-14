using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CompoundCellSelector : ICellSelector
    {
        [BoxGroup("Selector A", ShowLabel = false)]
        [GUIColor(1, 1, 1)]
        [ShowInInspector, OdinSerialize]
        public ICellSelector SelectorA { get; private set; }

        [BoxGroup("Operation", ShowLabel = false)]
        [GUIColor(0.8f, 0.8f, 0.8f)]
        [ShowInInspector, OdinSerialize]
        public BooleanOperation BooleanOperation { get; private set; }

        [BoxGroup("Selector B", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public ICellSelector SelectorB { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            //Optimization
            var selectionA = SelectorA.GetCellPositions(context);
            var selectionB = SelectorB.GetCellPositions(context);
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
