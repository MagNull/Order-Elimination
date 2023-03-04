using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination
{
    public abstract class Point : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _pathPrefab;
        public PointInfo PointInfo { get; protected set; }
        public PointView PointView { get; protected set; }
        public List<Point> NextPoints { get; protected set; }
        public int PointNumber { get; set; }
        public event Action<Point> OnSelected;
        private void Awake()
        {
            NextPoints = new List<Point>();
            PointView = new PointView(transform, _pathPrefab);
        }

        public virtual void Visit(Squad squad){}
        
        public void SetPlanetInfo(PointInfo pointInfo)
        {
            PointInfo = pointInfo;
        }

        public void SetNextPoints(IEnumerable<Point> paths)
        {
            NextPoints.AddRange(paths);
            foreach(var path in paths)
                PointView.SetPath(path.transform.position);
        }

        public void ShowPaths() => PointView.ShowPaths();

        private void OnMouseDown() => Select();

        public void Select()
        {
            Debug.Log("Select point");
            OnSelected?.Invoke(this);
        }
    }
}