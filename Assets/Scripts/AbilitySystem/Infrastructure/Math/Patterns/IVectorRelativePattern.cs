//using OrderElimination.AbilitySystem;
//using Sirenix.OdinInspector;
//using Sirenix.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace OrderElimination.Infrastructure
//{
//    [GUIColor(0.6f, 1, 0.6f)]
//    public interface IVectorRelativePattern
//    {
//        public Vector2Int[] GetAbsolutePositions(
//            Vector2Int startPoint, Vector2Int endPoint, CellRangeBorders calculatingBorders);
//    }

//    public class LinePattern : IVectorRelativePattern
//    {
//        [HideInInspector, OdinSerialize]
//        private float _minimalSquareThreshold;

//        [ShowInInspector, OdinSerialize]
//        public bool IncludeStart { get; set; }

//        [ShowInInspector, OdinSerialize]
//        public bool IncludeEnd { get; set; }

//        [ShowInInspector]
//        public float MinimalSquareThreshold
//        {
//            get => _minimalSquareThreshold;
//            set
//            {
//                if (value < 0) value = 0;
//                if (value > 0.5f) value = 0.5f;
//                _minimalSquareThreshold = value;
//            }
//        }

//        public Vector2Int[] GetAbsolutePositions(
//            Vector2Int startPoint, Vector2Int endPoint, CellRangeBorders calculatingBorders)
//        {
//            var intersections = IntersectionSolver.GetIntersections(startPoint, endPoint).ToArray();
//            var result = new List<Vector2Int>();
//            //var mapView = battleContext.AnimationSceneContext.BattleMapView;
//            //var textEmit = battleContext.AnimationSceneContext.TextEmitter;
//            //Debug.DrawLine(mapView.GameToWorldPosition(startPoint), mapView.GameToWorldPosition(endPoint), Color.green, 3, false);
//            foreach (var i in intersections)
//            {
//                Logging.Log($"Intersection: position={i.CellPosition}; square={i.SmallestPartSquare}; angle={i.IntersectionAngle}", Colorize.Magenta);
//                //var pos = mapView.GameToWorldPosition(new Vector3(i.CellPosition.x, i.CellPosition.y));
//                //textEmit.Emit($"{Math.Round(i.SmallestPartSquare, 4)}", Color.magenta, pos, Vector3.zero, 3, 0.65f);
//                if (i.SmallestPartSquare >= MinimalSquareThreshold)
//                    result.Add(i.CellPosition);
//            }
//            return result.ToArray();
//        }
//    }

//    public class PathPattern : IVectorRelativePattern
//    {
//        public Vector2Int[] GetAbsolutePositions(
//            Vector2Int startPoint, Vector2Int endPoint, CellRangeBorders calculatingBorders)
//        {
//            Pathfinding.PathExists(startPoint, endPoint, calculatingBorders, p => true, out var path);
//            return path;
//        }
//    }
//}
