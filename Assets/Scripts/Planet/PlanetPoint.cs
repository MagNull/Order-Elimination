using System.Collections.Generic;
using System;
using UnityEngine;


namespace OrderElimination
{
    public class PlanetPoint : MonoBehaviour, ISelectable
    {
        private PlanetInfo _planetInfo;
        private PlanetView _planetView;
        private List<Path> _paths;
        public event Action<PlanetPoint> Onclick;

        private void Awake()
        {
            _planetView = new PlanetView(transform);
            _paths = new List<Path>();
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

        public IReadOnlyList<PlanetPoint> GetNextPoints()
        {
            List<PlanetPoint> nextPoints = new List<PlanetPoint>();
            foreach(var path in _paths)
                nextPoints.Add(path.End);
            return nextPoints;
        }

        public void ShowPaths()
        {
            foreach (var path in _paths)
            {
                Debug.Log($"{gameObject.name}: showPath");
                path.gameObject.SetActive(true);
                path.Increase();
            }
        }

        public void HidePaths()
        {
            foreach (var path in _paths)
            {
                Debug.Log($"{gameObject.name}: hidePath");
                path.gameObject.SetActive(false);
                path.Decrease();
            } 
        }

        public void OnClick() => Onclick?.Invoke(this);

        public void Select(){}

        public void Unselect(){}
    }
}