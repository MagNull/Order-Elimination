using System.Collections.Generic;
using System;
using UnityEngine;


namespace OrderElimination
{
    public class PlanetPoint : MonoBehaviour, ISelectable
    {
        private PlanetInfo _planetInfo;
        private PlanetView _planetView;
        public bool isSelected;
        private List<Path> _paths;
        public event Action<PlanetPoint> Selected;
        public event Action<PlanetPoint> Unselected;

        private void Awake()
        {
            _planetView = new PlanetView(transform);
            _paths = new List<Path>();
            isSelected = false;
        }

        public PlanetInfo GetPlanetInfo() => _planetInfo;

        public void IncreasePoint() => _planetView.Increase();
        public void DecreasePoint() => _planetView.Decrease();

        public void MoveSquad(Squad squad)
        {
            squad.Move(this);
        }

        public void SetPath(Path path)
        {
            Debug.Log($"SetPath: {path.gameObject.name}");
            _paths.Add(path);
        }

        public void ShowPaths()
        {
            foreach (var path in _paths)
            {
                Debug.Log($"{gameObject.name}: showPath");
                path.gameObject.SetActive(true);
                path.IncreaseEndPoint();
            }
        }

        public void HidePaths()
        {
            foreach (var path in _paths)
            {
                Debug.Log($"{gameObject.name}: hidePath");
                path.gameObject.SetActive(false);
                path.DecreaseEndPoint();
            } 
        }

        public void Select()
        {
            Debug.Log($"{this.name}:isSelected = true");
            Selected?.Invoke(this);
            isSelected = true;
        }

        public void Unselect()
        {
            Debug.Log($"{this.name}: isSelected = false");
            Unselected?.Invoke(this);
            isSelected = false;
        }
    }
}