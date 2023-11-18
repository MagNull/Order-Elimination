using System;
using GameInventory.Items;
using OrderElimination;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameInventory.Views
{
    public class InventoryCellView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<IReadOnlyCell> Clicked;

        [SerializeField]
        private Image _iconRenderer;

        [SerializeField]
        private float _clickDistanceFault = 1f;
        private Vector2 _downPosition;

        private IReadOnlyCell _model;

        public IReadOnlyCell Model => _model;

        public void Init(IReadOnlyCell newCell)
        {
            _model = newCell;
            UpdateView();
        }

        public virtual void UpdateView()
        {
            if (_model.Item is EmptyItem || _model.Item.Data.HideInInventory)
                Disable();
            else
            {
                _iconRenderer.sprite = _model.Item.Data.View.Icon;
                Enable();
            }
        }

        public void Enable()
        {
            if (_model.Item.Data.HideInInventory)
                return;
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _downPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (Vector2.Distance(_downPosition, eventData.position) > _clickDistanceFault)
                return;

            Logging.Log("Click");
            Clicked?.Invoke(_model);
            _downPosition = Vector2.zero;
        }
    }
}