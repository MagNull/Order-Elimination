using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory
{
    public class InventoryCellView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _nameText;
        [SerializeField]
        private Image _iconRenderer;
        
        private Cell _cell;

        public void OnCellChanged(Cell newCell)
        {
            _cell = newCell;
            _nameText.text = _cell.Item.View.Name;
            _descriptionText.text = _cell.Item.View.Description;
            _iconRenderer.sprite = _cell.Item.View.Icon;
        }
    }
}