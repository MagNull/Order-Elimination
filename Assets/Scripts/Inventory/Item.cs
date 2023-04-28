using System;
using Sirenix.OdinInspector;

namespace Inventory_Items
{
    [Serializable]
    public class Item
    {
        [ShowInInspector]
        private readonly ItemView _itemView;
        private readonly ItemType _itemType;
        
        public ItemView View => _itemView;
        public ItemType Type => _itemType;
        
        public Item(ItemData itemData)
        {
            _itemView = itemData.ItemView;
            _itemType = itemData.ItemType;
        }

        public Item()
        {
            _itemView = new();
            _itemType = ItemType.Null;
        }
    }
}