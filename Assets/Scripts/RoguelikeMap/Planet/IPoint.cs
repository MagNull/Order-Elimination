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
    public abstract class IPoint : MonoBehaviour
    {
        [ShowInInspector]
        private int _countSquadOnPoint;
        [FormerlySerializedAs("_planetInfo")] [SerializeField]
        private PointInfo pointInfo;
        private PointView _pointView;
        private List<IPoint> _nextPoints;
        private int _pointNumber;
        public bool HasEnemy { get; private set; }
        public static event Action<IPoint> Onclick;

        public IReadOnlyList<IPoint> NextPoints => _nextPoints;
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
            _pointView = new PointView(transform);
            _nextPoints = new List<IPoint>();
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
        
        public void SetNextPoint(IPoint end)
        {
            _nextPoints.Add(end);
        }
        
        public void SetNextPoints(IEnumerable<IPoint> paths)
        {
            _nextPoints.AddRange(paths);
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
            Onclick?.Invoke(this);
        }
    }
}