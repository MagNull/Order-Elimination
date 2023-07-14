using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TakeCountCellSelector : ICellSelector
    {
        public enum TakeFromOption
        {
            FromStart,
            FromEnd
        }

        [ShowInInspector, OdinSerialize]
        public ICellSelector Source { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int Count { get; private set; }

        [ShowInInspector, OdinSerialize]
        public TakeFromOption TakeFrom { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var source = Source
                .GetCellPositions(context)
                .Where(p => context.BattleContext.BattleMap.ContainsPosition(p));
            return TakeFrom switch
            {
                TakeFromOption.FromStart => source.Take(Count).ToArray(),
                TakeFromOption.FromEnd => source.TakeLast(Count).ToArray(),
                _ => throw new NotImplementedException(),
            };


        }
    }
}
