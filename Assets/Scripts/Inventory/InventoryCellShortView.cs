using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryCellShortView : MonoBehaviour, IInventoryCellView
    {
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
    }
}