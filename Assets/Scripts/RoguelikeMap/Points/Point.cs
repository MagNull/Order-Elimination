using System;
using System.Threading.Tasks;
using RoguelikeMap.Panels;
using RoguelikeMap.Points.Models;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace RoguelikeMap.Points
{
    public class Point : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _background;
        [SerializeField]
        private SpriteRenderer _icon;
        private TransferPanel _transferPanel;
        private PanelManager _panelManager;
        private bool _isActive;
        
        public PointModel Model { get; private set; }

        [Inject]
        private void Construct(PanelManager panelManager, TransferPanel transferPanel)
        {
            _panelManager = panelManager;
            _transferPanel = transferPanel;
        }

        public void Initialize(PointModel model)
        {
            if (model is null)
                throw new ArgumentException("PointModel is null");
            Model = model;
            Model.OnChangeActivity += SetActive;
            _icon.sprite = model.Sprite;
            Model.SetPanel(_panelManager);
        }

        public async Task Visit(Squad squad) => await Model.Visit(squad);

        private void SetActive(bool isActive)
        {
            _isActive = isActive;
            var color = isActive ? Color.white : Color.gray;
             _background.color = color;
            if (_icon is not null)
                _icon.color = color;
        }

        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject() && _isActive && Model is not StartPointModel)
                _transferPanel.Initialize(this);
        }

        private void OnDestroy()
        {
            Model.OnChangeActivity -= SetActive;
        }
    }
}