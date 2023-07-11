using System;
using System.Collections.Generic;
using OrderElimination;
using RoguelikeMap.Panels;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace RoguelikeMap.Points
{
    public class Point : MonoBehaviour
    {
        private PanelManager _panelManager;
        [SerializeField]
        private Button _button;
     
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

        private void Awake()
        {
            _button.onClick.AddListener(Select);
        }

        public void SetPointModel(PointModel pointModel, Sprite pointIcon)
        {
            Model = pointModel ?? throw new ArgumentException("PointModel is null");
            Model.SetPanel(_panelManager);
            _button.image.sprite = pointIcon;
        }

        public void Visit(Squad squad) => Model.Visit(squad);

        private void Select()
        {
            OnSelected?.Invoke(this);
        }
    }
}