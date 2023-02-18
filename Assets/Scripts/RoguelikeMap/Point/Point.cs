using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        public void IncreasePoint() => PointView.Increase();
        public void DecreasePoint() => PointView.Decrease();

        public virtual void Visit(Squad squad){}

        // public void SetEnemy(bool hasEnemy)
        // {
        //     HasEnemy = hasEnemy;
        //     AddSquad();
        // }

        public void SetPlanetInfo(PointInfo pointInfo)
        {
            PointInfo = pointInfo;
        }

        public void SetNextPoint(Point end)
        {
            NextPoints.Add(end);
        }

        public void SetNextPoints(IEnumerable<Point> paths)
        {
            NextPoints.AddRange(paths);
            foreach(var path in paths)
                PointView.SetPath(path.transform.position);
        }

        public void ShowPaths() => PointView.ShowPaths();

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
            Debug.Log("Select point");
            OnSelected?.Invoke(this);
        }
    }
}