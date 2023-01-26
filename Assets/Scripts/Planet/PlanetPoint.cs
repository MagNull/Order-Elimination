using System.Collections.Generic;
using System;
using System.Linq;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OrderElimination
{
    public class PlanetPoint : MonoBehaviour, ISelectable
    {
        [ShowInInspector]
        private int _countSquadOnPoint;
        [SerializeField]
        private PlanetInfo _planetInfo;
        private PlanetView _planetView;
        private List<Path> _paths;
        private int _pointNumber;
        public bool HasEnemy { get; private set; }
        public static event Action<PlanetPoint> Onclick;

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
            _planetView = new PlanetView(transform);
            _paths = new List<Path>();
        }

        public PlanetInfo GetPlanetInfo() => _planetInfo;

        public void IncreasePoint() => _planetView.Increase();
        public void DecreasePoint() => _planetView.Decrease();
        
        public void RemoveSquad() => _countSquadOnPoint--;
        public void AddSquad() => _countSquadOnPoint++;

        public void SetEnemy(bool hasEnemy)
        {
            HasEnemy = hasEnemy;
            AddSquad();
        }

        public void SetPlanetInfo(PlanetInfo planetInfo)
        {
            _planetInfo = planetInfo;
        }
        
        public void SetPath(Path path)
        {
            _paths.Add(path);
        }
        
        public void SetPath(IEnumerable<Path> paths)
        {
            _paths.AddRange(paths);
        }

        public IReadOnlyList<PlanetPoint> GetNextPoints()
        {
            List<PlanetPoint> nextPoints = new List<PlanetPoint>();
            foreach(var path in _paths)
                nextPoints.Add(path.EndPoint);
            return nextPoints;
        }

        public void ShowPaths()
        {
            foreach (var path in _paths)
            {
                path.ActivateSprite(true);
            }
        }

        public void HidePaths()
        {
            foreach (var path in _paths.Where(path => !path.IsDestroyed()))
            {
                path.ActivateSprite(false);
            }
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

        public void Unselect()
        {
            throw new NotImplementedException();
        }
    }
}