using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Points
{
    public class Point : MonoBehaviour
    {
        private PanelManager _panelManager;
     
        public PointModel Model { get; private set; }
        public event Action<Point> OnSelected;
        public IReadOnlyList<int> NextPoints => Model.NextPoints;
        public int Index => Model.Index;
        public bool IsLastPoint => Model.IsLastPoint;

        [Inject]
        private void Construct(PanelManager panelManager)
        {
            _panelManager = panelManager;
        }

        public void SetPointModel(PointModel pointModel)
        {
            Model = pointModel ?? throw new ArgumentException("PointModel is null");
            Model.SetPanel(_panelManager);
        }

        public void Visit(Squad squad) => Model.Visit(squad);

        private void OnMouseDown() => Select();

        private void Select()
        {
            OnSelected?.Invoke(this);
        }
    }
}