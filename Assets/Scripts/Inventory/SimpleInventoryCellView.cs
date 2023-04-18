using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory_Items
{
    public class SimpleInventoryCellView : MonoBehaviour, IInventoryCellView, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<IReadOnlyCell> Clicked;
        
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private Image _iconRenderer;

        [SerializeField]
        private float _clickDistanceFault = 1f;
        private Vector2 _downPosition;
        
        private IReadOnlyCell _cell;

        public void OnCellChanged(IReadOnlyCell newCell)
        {
            if (newCell.ItemQuantity == 0)
            {
                Destroy(gameObject);
                return;
            }
            _cell = newCell;
            _nameText.text = _cell.Item.View.Name;
            _descriptionText.text = _cell.Item.View.Description;
            _iconRenderer.sprite = _cell.Item.View.Icon;
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
            
            Debug.Log("Click");
            Clicked?.Invoke(_cell);
            _downPosition = Vector2.zero;
        }
    }
}