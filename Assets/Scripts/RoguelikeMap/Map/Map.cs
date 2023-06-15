using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace RoguelikeMap.Map
{
    public class Map : MonoBehaviour
    {
        public static string SquadPositionPrefPath = $"{SaveIndex}/Squad/Position";
        
        private IEnumerable<Point> _points;
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private bool _isSquadSelected;
        private IObjectResolver _objectResolver;
        public static int SaveIndex { get; private set; }

        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad, IObjectResolver objectResolver)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
            _objectResolver = objectResolver;
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
        
        private void UnselectSquad()
        {
            _isSquadSelected = false;
        }

        private void SelectPoint(Point point)
        {
            if (_isSquadSelected is false)
                return;
            if(_squad.Point.NextPoints.Contains(point.Index))
                point.Visit(_squad);
            UnselectSquad();
        }

        private void SetSquadPosition()
        {
            var position = PlayerPrefs.HasKey(SquadPositionPrefPath)
                ? PlayerPrefs.GetString(SquadPositionPrefPath).GetVectorFromString()
                : _points.First().transform.position;
            var nearestPoint = FindNearestPoint(position);
            _squad.Visit(nearestPoint.Model);
        }

        private Point FindNearestPoint(Vector3 position)
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

        public void LoadStartScene()
        {
            var sceneTransition = _objectResolver.Resolve<SceneTransition>();
            sceneTransition.LoadStartSessionMenu();
        }
    }
}