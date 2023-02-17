using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace OrderElimination
{
    public abstract class Point : MonoBehaviour
    {
        [ShowInInspector]
        private int _countSquadOnPoint;
        private PointInfo pointInfo;
        private PointView _pointView;
        private List<Point> _nextPoints;
        private int _pointNumber;
        [SerializeField] 
        private GameObject _linePrefab;
        public bool HasEnemy { get; private set; }
        public static event Action<Point> Onclick;

        public IReadOnlyList<Point> NextPoints => _nextPoints;
        public int CountSquadOnPoint => _countSquadOnPoint;

        public int PointNumber
        {
            get => _pointNumber;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Planet point < 0");
                _pointNumber = value;
            }
        }

        private void Awake()
        {
            _pointView = new PointView(transform, _linePrefab);
            _nextPoints = new List<Point>();
        }

        public PointInfo GetPlanetInfo() => pointInfo;

        public void IncreasePoint() => _pointView.Increase();
        public void DecreasePoint() => _pointView.Decrease();
        
        public void RemoveSquad() => _countSquadOnPoint--;
        public void AddSquad() => _countSquadOnPoint++;

        public void SetEnemy(bool hasEnemy)
        {
            HasEnemy = hasEnemy;
            AddSquad();
        }

        public void SetPlanetInfo(PointInfo pointInfo)
        {
            this.pointInfo = pointInfo;
        }
        
        public void SetNextPoint(Point end)
        {
            _nextPoints.Add(end);
            _pointView.SetPath(end.transform.position);
        }
        
        public void SetNextPoints(IEnumerable<Point> paths)
        {
            _nextPoints.AddRange(paths);
            foreach (var path in paths)
            {
                _pointView.SetPath(path.transform.position);
            }
        }

        public void ShowPaths()
        {
            _pointView.ShowPaths();
        }

        public void HidePaths()
        {
            _pointView.HidePaths();
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
            Onclick?.Invoke(this);
        }
    }
}