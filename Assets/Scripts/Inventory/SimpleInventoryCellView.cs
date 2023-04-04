using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory_Items
{
    public class SimpleInventoryCellView : MonoBehaviour, IInventoryCellView, IPointerDownHandler
    {
        public event Action<IReadOnlyCell> Clicked;
        
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private Image _iconRenderer;
        
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
            Debug.Log("Click");
            Clicked?.Invoke(_cell);
        }
    }
}