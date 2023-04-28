using System;
using System.Collections.Generic;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using UnityEngine;

namespace RoguelikeMap.Points
{
    public class Point : MonoBehaviour
    {
        private PanelGenerator _panelGenerator;
        private PointView _pointView;
        
        public PointModel Model { get; private set; }
        public IReadOnlyList<int> NextPoints => Model.NextPoints;
        public int PointIndex => Model.Index;

        public event Action<Point> OnSelected;

        #region SetParameters

        public void SetPointModel(PointModel pointModel)
        {
            Model = pointModel ?? throw new ArgumentException("PointModel is null");
        }
        
        public void SetPanelGenerator(PanelGenerator panelGenerator)
        {
            _panelGenerator = panelGenerator ?? throw new ArgumentException("PanelGenerator is null");
        }

        #endregion
        
        //When squad come to point
        private void InitializePointView()
        {
            var panel = _panelGenerator.GetPanelByPointInfo(Model.Type);
            _pointView = new PointView(panel);
            _pointView.SetActivePanel(true);
        }

        public void Visit(Squad squad)
        {
            squad.Visit(this);
            if(_pointView is null)
                InitializePointView();
        }

        #region Events

        private void OnMouseDown() => Select();

        private void Select()
        {
            Debug.Log("Select point");
            OnSelected?.Invoke(this);
        }

        #endregion
    }
}