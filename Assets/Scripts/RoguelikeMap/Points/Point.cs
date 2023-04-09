using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Points
{
    public abstract class Point : MonoBehaviour
    {
        [SerializeField]
        private GameObject _pathPrefab;
        
        protected PanelGenerator _panelGenerator;
        
        public PointInfo PointInfo { get; protected set; }
        public PathView PathView { get; protected set; }
        public PointView PointView { get; protected set; }
        public List<Point> NextPoints { get; protected set; }
        public int PointNumber { get; set; }
        public event Action<Point> OnSelected;
        
        private void Awake()
        {
            NextPoints = new List<Point>();
            PathView = new PathView(transform, _pathPrefab);
        }
        
        //When squad come to point
        public virtual void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointInfo.PointType);
            PointView = new PointView(panel);
            PointView.SetActivePanel(true);
        }

        public virtual void Visit(Squad squad)
        {
            squad.Visit(this);
            if(PointView is null)
                InitializePointView();
        }
        
        public void SetPointInfo(PointInfo pointInfo)
        {
            PointInfo = pointInfo;
        }

        public void SetPanelGenerator(PanelGenerator panelGenerator)
        {
            _panelGenerator = panelGenerator;
        }

        public void SetNextPoints(IEnumerable<Point> paths)
        {
            NextPoints.AddRange(paths);
            foreach(var path in paths)
                PathView.SetPath(path.transform.position);
        }

        public void ShowPaths() => PathView.ShowPaths();

        private void OnMouseDown() => Select();

        public void Select()
        {
            Debug.Log("Select point");
            OnSelected?.Invoke(this);
        }
    }
}