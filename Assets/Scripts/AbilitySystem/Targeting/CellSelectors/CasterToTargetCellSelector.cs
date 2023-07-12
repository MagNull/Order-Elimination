using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class CasterToTargetCellSelector : ICellSelector
    {
        public enum TrajectoryType
        {
            Line = 1,
            //+ MinIntersectionSquareToInclude
            Ray = 2,
            //+ MinIntersectionSquareToInclude, DirectionAngleOffset(0 => c->t, 180 => t->c) + Offsets
            Path = 4,
            //+ PathConditions ?
        }

        [HideInInspector, OdinSerialize]
        private float _minIntersectionSquareToInclude;

        [ShowInInspector, OdinSerialize]
        public TrajectoryType Trajectory { get; private set; } = TrajectoryType.Line;

        [ShowIf("@Trajectory == TrajectoryType.Line || Trajectory == TrajectoryType.Ray")]
        [ShowInInspector]
        public float MinIntersectionSquareToInclude
        {
            get => _minIntersectionSquareToInclude;
            set
            {
                if (value < 0) value = 0;
                if (value > 0.5f) value = 0.5f;
                _minIntersectionSquareToInclude = value;
            }
        }

        [ShowIf("@Trajectory == TrajectoryType.Line || Trajectory == TrajectoryType.Ray")]
        [ShowInInspector, OdinSerialize]
        public ICellCondition[] TakeWhileConditions { get; private set; } = new ICellCondition[0];

        [ShowIf("@Trajectory == TrajectoryType.Path")]
        [ShowInInspector, OdinSerialize]
        public ICellCondition[] PathConditions { get; private set; } = new ICellCondition[0];

        public Vector2Int[] GetCellPositions(CellSelectorContext context)
        {
            var battleContext = context.BattleContext;
            var askingEntity = context.AskingEntity;
            var result = new List<Vector2Int>();
            foreach (var targetPos in context.SelectedCellPositions)
            {
                if (Trajectory == TrajectoryType.Line
                    || Trajectory == TrajectoryType.Ray)
                {
                    CellIntersection[] intersections;
                    if (Trajectory == TrajectoryType.Line)
                    {
                        intersections = IntersectionSolver.GetIntersections(
                            askingEntity.Position, targetPos).ToArray();
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                    result.AddRange(intersections
                        .Where(i => i.SmallestPartSquare >= MinIntersectionSquareToInclude)
                        .TakeWhile(i => TakeWhileConditions.All(
                            c => c.IsConditionMet(battleContext, askingEntity, i.CellPosition)))
                        .Select(i => i.CellPosition));
                }
                else if (Trajectory == TrajectoryType.Path)
                {
                    if (Pathfinding.PathExists(
                        askingEntity.Position,
                        targetPos,
                        battleContext.BattleMap.CellRangeBorders,
                        p => PathConditions.All(c => c.IsConditionMet(battleContext, askingEntity, p)),
                        out var path))
                        result.AddRange(path);
                }
                else
                    throw new NotImplementedException();
            }
            return result.ToArray();
        }
    }
}
