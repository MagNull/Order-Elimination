using GameInventory.Items;
using TMPro;
using UnityEngine;

namespace GameInventory.Views
{
    public class FullInventoryCellView : InventoryCellView
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionText;
        [SerializeField]
        private TextMeshProUGUI _nameText;

        public override void UpdateView()
        {
            base.UpdateView();
            if(Model.Item is EmptyItem)
                return;
            _nameText.text = Model.Item.Data.View.Name;
            _descriptionText.text = Model.Item.Data.View.Description;
        }
    }
}