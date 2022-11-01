using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace OrderElimination
{
    public class PlanetPoint : MonoBehaviour, ISelectable
    {
        private PlanetInfo _planetInfo;
        private PlanetView _planetView;
        public bool isSelected;
        
        [SerializeField] private List<Path> _paths;

        private void Awake()
        {
            _planetView = new PlanetView(transform);
            isSelected = false;
        }

        public PlanetInfo GetPlanetInfo() => _planetInfo;

        public void IncreasePoint() => _planetView.Increase();
        public void DecreasePoint() => _planetView.Decrease();

        public void MoveSquad(Squad squad)
        {
            squad.Move(this);
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

        public PlanetPoint GetSelectedPath()
        {
            foreach (var path in _paths)
            {
                var planetPoint = path.GetSelectedEndPoint();
                Debug.Log(planetPoint);
                if (planetPoint != null)
                    return planetPoint;
            }
            return null;
        }

        public void Select()
        {
            Debug.Log($"{this.name}:isSelected = true");
            isSelected = true;
        }

        public void Unselect()
        {
            Debug.Log($"{this.name}: isSelected = false");
            isSelected = false;
        }
    }
}