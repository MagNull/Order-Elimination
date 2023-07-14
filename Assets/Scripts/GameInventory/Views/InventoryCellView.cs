using System;
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
        
        private IReadOnlyCell _cell;

        public virtual void Init(IReadOnlyCell newCell)
        {
            _cell = newCell;
            _iconRenderer.sprite = _cell.Item.Data.View.Icon;
        }

        public void Enable()
        {
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
            if(Vector2.Distance(_downPosition, eventData.position) > _clickDistanceFault)
                return;
            
            Logging.Log("Click");
            Clicked?.Invoke(_cell);
            _downPosition = Vector2.zero;
        }
    }
}