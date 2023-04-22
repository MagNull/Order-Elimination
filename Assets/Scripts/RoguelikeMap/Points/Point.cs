using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.Points.VarietiesPoints.Infos;
using UnityEngine;

namespace RoguelikeMap.Points
{
    public abstract class Point : MonoBehaviour
    {
        private LineRenderer _pathPrefab;
        protected PanelGenerator _panelGenerator;
        
        public VarietiesPointInfo PointInfoInfo { get; protected set; }
        public PathView PathView { get; protected set; }
        public PointView PointView { get; protected set; }
        public List<Point> NextPoints { get; protected set; } = new List<Point>();
        public int PointNumber { get; set; }
        public event Action<Point> OnSelected;

        //When squad come to point
        protected virtual void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(PointInfoInfo.PointType);
            PointView = new PointView(panel);
            PointView.SetActivePanel(true);
        }

        public virtual void Visit(Squad squad)
        {
            squad.Visit(this);
            if(PointView is null)
                InitializePointView();
        }
        
        public void SetPointInfo(VarietiesPointInfo pointInfoInfo)
        {
            PointInfoInfo = pointInfoInfo ?? throw new ArgumentException("PointInfo is null");
        }

        public void SetPanelGenerator(PanelGenerator panelGenerator)
        {
            _panelGenerator = panelGenerator ?? throw new ArgumentException("PanelGenerator is null");
        }

        public void SetPathPrefab(LineRenderer pathPrefab)
        {
            _pathPrefab = pathPrefab ?? throw new ArgumentException("pathPrefab is null");
            PathView = new PathView(transform, _pathPrefab);
        }

        public void SetNextPoints(IEnumerable<Point> paths)
        {
            NextPoints.AddRange(paths);
            foreach(var path in paths)
                PathView.SetPath(path.transform.position);
        }

        public void ShowPaths() => PathView.ShowPaths();

        private void OnMouseDown() => Select();

        private void Select()
        {
            Debug.Log("Select point");
            OnSelected?.Invoke(this);
        }
    }
}