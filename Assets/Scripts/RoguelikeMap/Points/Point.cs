using System;
using System.Collections.Generic;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using VContainer;

namespace RoguelikeMap.Points
{
    public class Point : MonoBehaviour
    {
        private PanelGenerator _panelGenerator;
     
        public PointModel Model { get; private set; }
        public event Action<Point> OnSelected;
        public IReadOnlyList<int> NextPoints => Model.NextPoints;
        public int Index => Model.Index;

        [Inject]
        private void Construct(PanelGenerator panelGenerator)
        {
            _panelGenerator = panelGenerator;
        }

        public void SetPointModel(PointModel pointModel)
        {
            Model = pointModel ?? throw new ArgumentException("PointModel is null");
            Model.SetPanel(_panelGenerator);
        }

        public void Visit(Squad squad) => Model.Visit(squad);

        private void OnMouseDown() => Select();

        private void Select()
        {
            Debug.Log("Select point");
            OnSelected?.Invoke(this);
        }
    }
}