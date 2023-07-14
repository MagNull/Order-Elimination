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
        [SerializeField]
        private SpriteRenderer _spriteRenderer;
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

        public void Initialize(PointInfo info)
        {
            Model = info.Model ?? throw new ArgumentException("PointModel is null");
            _spriteRenderer.sprite = info.PointSprite;
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