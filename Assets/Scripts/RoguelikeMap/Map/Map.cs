using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using VContainer;
using Point = RoguelikeMap.Points.Point;

namespace RoguelikeMap.Map
{
    public class Map : MonoBehaviour
    {
        public static string SquadPositionPrefPath = $"{SaveIndex}/Squad/Position";
        
        public IEnumerable<Point> _points;
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private bool _isSquadSelected;
        public static int SaveIndex { get; private set; }

        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
        }

        private void Start()
        {
            _points = _mapGenerator.GenerateMap();
            SetSquadPosition();
            foreach (var point in _points)
                point.OnSelected += SelectPoint;

            _squad.OnSelected += SelectSquad;
        }
        
        private void SelectSquad(Squad squad)
        {
            _isSquadSelected = true;
        }
        
        public void UnselectSquad()
        {
            _isSquadSelected = false;
        }

        private void SelectPoint(Point point)
        {
            if (_isSquadSelected is false)
                return;
            if(_squad.Point.NextPoints.Contains(point))
                point.Visit(_squad);
            UnselectSquad();
        }

        private void SetSquadPosition()
        {
            var position = PlayerPrefs.HasKey(SquadPositionPrefPath)
                ? PlayerPrefs.GetString(SquadPositionPrefPath).GetVectorFromString()
                : _points.First().transform.position;
            var nearestPoint = FindNearestPoint(position);
            _squad.Move(nearestPoint);
        }

        public Point FindNearestPoint(Vector3 position)
        {
            Point nearestPoint = null;
            var minDistance = double.MaxValue;
            foreach (var point in _points)
            {
                var distance = Vector3.Distance(position, point.transform.position);
                if (!(minDistance > distance)) continue;
                minDistance = distance;
                nearestPoint = point;
            }

            return nearestPoint;
        }

        public void ReloadMap()
        {
            PlayerPrefs.DeleteKey(SquadPositionPrefPath);
            SetSquadPosition();
        }
    }
}