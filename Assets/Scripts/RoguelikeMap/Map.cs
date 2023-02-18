using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using UnityEngine;
using VContainer;

namespace RoguelikeMap
{
    public class Map : MonoBehaviour
    {
        public string SquadPositionPrefPath = $"{SaveIndex}/Squad/Position";
        
        public List<OrderElimination.Point> _points;
        private IMapGenerator _mapGenerator;
        private Squad _squad;
        private bool _isSquadSelected;
        public static int SaveIndex { get; private set; }

        public static event Action<List<OrderElimination.Point>> OnShowPath;
        
        [Inject]
        private void Construct(IMapGenerator mapGenerator, Squad squad)
        {
            _mapGenerator = mapGenerator;
            _squad = squad;
        }

        private void Start()
        {
            //_points = _mapGenerator.GenerateMap();
            OnShowPath?.Invoke(_points);
            //SetSquadPosition();
            for(var i = 0; i < _points.Count; i++)
            {
                _points[i].ShowPaths();
                _points[i].OnSelected += SelectPoint;
                if (i == _points.Count - 1)
                    break;
                _points[i].SetNextPoint(_points[i + 1]);
            }

            _squad.Move(_points.First());
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

        private void SelectPoint(OrderElimination.Point point)
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
            _squad.Move(position);
        }
    }
}