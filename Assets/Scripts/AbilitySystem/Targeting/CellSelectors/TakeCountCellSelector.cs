using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TakeCountCellSelector : ICellSelector
    {
        public enum TakeOption
        {
            FromStart,
            FromEnd,
            FromRandomIndex
        }

        [ShowInInspector, OdinSerialize]
        public ICellSelector Source { get; private set; }

        [ShowInInspector, OdinSerialize]
        public int Count { get; private set; }

        [ShowInInspector, OdinSerialize]
        public TakeOption TakeFrom { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var source = Source
                .GetCellPositions(context)
                .Where(p => context.BattleContext.BattleMap.ContainsPosition(p));
            return TakeFrom switch
            {
                TakeOption.FromStart => source.Take(Count).ToArray(),
                TakeOption.FromEnd => source.TakeLast(Count).ToArray(),
                TakeOption.FromRandomIndex => TakeRandomCount(source, Count),
                _ => throw new NotImplementedException(),
            };
        }

        private Vector2Int[] TakeRandomCount(IEnumerable<Vector2Int> source, int count)
        {
            var remainingPositions = source.ToList();
            if (remainingPositions.Count == 0)
                return Enumerable.Empty<Vector2Int>().ToArray();
            var result = new List<Vector2Int>();
            for (var i = 0; i < count; i++)
            {
                if (remainingPositions.Count == 0)
                    break;
                var randomId = UnityEngine.Random.Range(0, remainingPositions.Count);
                result.Add(remainingPositions[randomId]);
                remainingPositions.RemoveAt(randomId);
            }
            return result.ToArray();
        }
    }
}
