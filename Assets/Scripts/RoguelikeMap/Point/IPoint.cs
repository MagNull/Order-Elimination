﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OrderElimination
{
    public interface IPoint
    {
        public PointInfo PointInfo { get; protected set; }
        public PointView PointView { get; protected set; }
        public List<IPoint> NextPoints { get; protected set; }
        public int PointNumber { get; protected set; }

        // private void Awake()
        // {
        //     _pointView = new PointView(transform);
        //     _nextPoints = new List<IPoint>();
        // }

        public void IncreasePoint() => PointView.Increase();
        public void DecreasePoint() => PointView.Decrease();

        public void Visit(Squad squad)
        {
            throw new NotImplementedException();
        }

        // public void SetEnemy(bool hasEnemy)
        // {
        //     HasEnemy = hasEnemy;
        //     AddSquad();
        // }

        public void SetPlanetInfo(PointInfo pointInfo)
        {
            PointInfo = pointInfo;
        }
        
        public void SetNextPoint(IPoint end)
        {
            NextPoints.Add(end);
        }
        
        public void SetNextPoints(IEnumerable<IPoint> paths)
        {
            NextPoints.AddRange(paths);
        }

        public void ShowPaths()
        {
            // foreach (var path in _paths)
            // {
            //     path.ActivateSprite(true);
            // }
        }

        public void HidePaths()
        {
            // foreach (var path in _paths.Where(path => !path.IsDestroyed()))
            // {
            //     path.ActivateSprite(false);
            // }
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("Blocked");
                return;
            }
            Select();
        }

        public void Select()
        {
            // Onclick?.Invoke(this);
        }
    }
}