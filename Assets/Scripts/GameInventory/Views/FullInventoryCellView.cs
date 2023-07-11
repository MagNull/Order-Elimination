using System;
using TMPro;
using UnityEngine;

namespace GameInventory.Views
{
    public class FullInventoryCellView : InventoryCellView
    {
        public event Action<IReadOnlyCell> Clicked;
        
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _nameText;

        public override void Init(IReadOnlyCell newCell)
        {
            base.Init(newCell);
            _nameText.text = newCell.Item.View.Name;
            _descriptionText.text = newCell.Item.View.Description;
        }
    }
}