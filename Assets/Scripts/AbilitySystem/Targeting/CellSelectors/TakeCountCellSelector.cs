using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class TakeCountCellSelector : ICellSelector
    {
        public enum TakePriorityOption
        {
            FromStart,
            FromEnd,
            ClosestToCaster,
            FurthestFromCaster,
            ClosestToTarget,
            FurthestFromTarget
        }

        [ShowInInspector, OdinSerialize]
        public int Count { get; private set; }

        [ShowInInspector, OdinSerialize]
        public ICellSelector Source { get; private set; }

        [ShowInInspector, OdinSerialize]
        public TakePriorityOption TakePriority { get; private set; }

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var source = Source.GetCellPositions(context);
            if (TakePriority == TakePriorityOption.FromStart)
                return source.Take(Count).ToArray();
            if (TakePriority == TakePriorityOption.FromEnd)
                return source.TakeLast(Count).ToArray();
            if (TakePriority == TakePriorityOption.ClosestToCaster
                || TakePriority == TakePriorityOption.FurthestFromCaster)
            {
                var orderedFromCaster = source.OrderBy(p => (p - context.AskingEntity.Position).sqrMagnitude);
                return TakePriority switch
                {
                    TakePriorityOption.ClosestToCaster => orderedFromCaster.Take(Count).ToArray(),
                    TakePriorityOption.FurthestFromCaster => orderedFromCaster.TakeLast(Count).ToArray(),
                    _ => throw new InvalidProgramException()
                };
            }
            if (TakePriority == TakePriorityOption.ClosestToTarget
                || TakePriority == TakePriorityOption.FurthestFromTarget)
            {
                if (context.SelectedCellPositions.Length == 0)
                    return new Vector2Int[0];
                if (context.SelectedCellPositions.Length == 1)
                {
                    var target = context.SelectedCellPositions[0];
                    var orderedFromTarget = source.OrderBy(p => (p - target).sqrMagnitude);
                    return TakePriority switch
                    {
                        TakePriorityOption.ClosestToTarget => orderedFromTarget.Take(Count).ToArray(),
                        TakePriorityOption.FurthestFromTarget => orderedFromTarget.TakeLast(Count).ToArray(),
                        _ => throw new InvalidProgramException()
                    };
                }
                throw new NotImplementedException("Support for multi-selection is to be added.");
            }
            throw new NotImplementedException();
        }
    }
}
