using System;
using UnityEngine;

namespace OrderElimination
{
    public class Path : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        
        private PlanetPoint _startPoint;
        private PlanetPoint _endPoint;
        public PlanetPoint StartPoint => _startPoint;
        public PlanetPoint EndPoint => _endPoint;

        private void Start()
        {
            _lineRenderer.SetWidth(7, 7);
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.SetColors(Color.black, Color.black);
        }

        public void ActivateSprite(bool isActive)
        {
            if (isActive)
            {
                _lineRenderer.sortingOrder = 2;
                _lineRenderer.SetColors(Color.yellow, Color.yellow);
            }
            else
            {
                _lineRenderer.sortingOrder = 1;
                _lineRenderer.SetColors(Color.black, Color.black);
            }
        }

        public void SetStartPoint(PlanetPoint planetPoint)
        {
            _startPoint = planetPoint;
            _lineRenderer.SetPosition(0, StartPoint.transform.position);
        }

        public void SetEndPoint(PlanetPoint planetPoint)
        {
            _endPoint = planetPoint;
            _lineRenderer.SetPosition(1, EndPoint.transform.position);
        }

        public void Increase()
        {
            _startPoint.IncreasePoint();
            _endPoint.IncreasePoint();
        }

        public void Decrease()
        {
            _startPoint.DecreasePoint();
            _endPoint.DecreasePoint();
        }
    }
}