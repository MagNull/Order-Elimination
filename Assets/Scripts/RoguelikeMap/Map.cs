﻿using System;
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
        [SerializeField] 
        private ClickProcess _backgroundClick;
        
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
            _points = _mapGenerator.GenerateMap();
            OnShowPath?.Invoke(_points);
            SetSquadPosition();
            foreach (var point in _points)
                point.OnSelected += SelectPoint;

            _backgroundClick.OnClick += UnselectSquad;
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
            var nearestPoint = FindNearestPoint(position);
            _squad.Move(nearestPoint);
        }

        public OrderElimination.Point FindNearestPoint(Vector3 position)
        {
            OrderElimination.Point nearestPoint = null;
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
    }
}