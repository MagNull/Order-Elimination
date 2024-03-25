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
        [SerializeField]
        private PathView _pathPrefab;
        private PathView _pathView;
        private TransferPanel _transferPanel;
        private PanelManager _panelManager;
        private Squad _squad;

        public PointModel Model { get; private set; }
        public bool IsInteractable { get; private set; }
        public Guid Id => Model.AssetId;

        [Inject]
        private void Construct(PanelManager panelManager, TransferPanel transferPanel,
            Squad squad)
        {
            _panelManager = panelManager;
            _transferPanel = transferPanel;
            _squad = squad;
        }

        public void Initialize(PointModel model, int index)
        {
            if (model is null)
                throw new ArgumentException("PointModel is null");
            Model = model;
            Model.OnChangeActivity += SetActive;
            _icon.sprite = model.Sprite;
            Model.SetPanel(_panelManager, _transferPanel);
            Model.SetIndex(index);
            _pathView = Instantiate(_pathPrefab, transform);
        }

        public async Task Visit(Squad squad) => await Model.Visit(squad);

        public void SetActive(bool isActive)
        {
            SetInterectable(isActive);
            var color = isActive ? Color.white : Color.gray;
            _background.color = color;
            if (_icon is not null)
                _icon.color = color;
        }

        public void SetInterectable(bool isInteractable)
        {
            IsInteractable = isInteractable;
        }

        public void ShowPaths() => SetActivePaths(true);

        public void HidePaths() => SetActivePaths(false);

        private void SetActivePaths(bool isActive)
        {
            SetActive(isActive);
            if (isActive)
                _pathView.UpdatePaths(this);
            else
                _pathView.ClearPaths();
            var nextPoints = Model.GetNextPoints();
            if (nextPoints is null)
                return;
            foreach (var pointModel in nextPoints)
                pointModel.SetActive(isActive);
        }

        private void OnMouseDown()
        {
            if (!EventSystem.current.IsPointerOverGameObject() && IsInteractable && Model is not StartPointModel)
                Model.ShowPreview(_squad);
        }

        private void OnDestroy()
        {
            Model.OnChangeActivity -= SetActive;
        }
    }
}