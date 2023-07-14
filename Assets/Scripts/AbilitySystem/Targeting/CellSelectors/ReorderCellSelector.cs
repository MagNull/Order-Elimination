using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ReorderCellSelector : ICellSelector
    {
        public enum OrderByOption
        {
            Nothing,
            DistanceFromCaster,
            DistanceFromTarget
        }

        [ShowInInspector, OdinSerialize]
        public OrderByOption OrderBy { get; private set; }

        [ShowInInspector, OdinSerialize]
        public bool Reverse { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ICellSelector Source { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var map = context.BattleContext.BattleMap;
            var source = Source.GetCellPositions(context).Where(p => map.ContainsPosition(p));
            IEnumerable<Vector2Int> orderedResult;
            switch (OrderBy)
            {
                case OrderByOption.Nothing:
                    orderedResult = source;
                    break;
                case OrderByOption.DistanceFromCaster:
                    orderedResult = source.OrderBy(p => (p - context.AskingEntity.Position).sqrMagnitude);
                    break;
                case OrderByOption.DistanceFromTarget:
                    if (context.SelectedCellPositions.Length == 0)
                        throw new ArgumentException("No targets for sorting.");
                    if (context.SelectedCellPositions.Length == 1)
                    {
                        var target = context.SelectedCellPositions[0];
                        orderedResult = source.OrderBy(p => (p - target).sqrMagnitude);
                        break;
                    }
                    throw new NotImplementedException("Support for multi-selection is to be added.");
                    //...
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Reverse ? orderedResult.Reverse().ToArray() : orderedResult.ToArray();
        }
    }
}
