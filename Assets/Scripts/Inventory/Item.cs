using System;
using Inventory;
using Sirenix.OdinInspector;

namespace Inventory_Items
{
    [Serializable]
    public class Item
    {
        [ShowInInspector]
        private readonly ItemView _itemView;
        private readonly ItemType _itemType;
        private readonly int _itemId;
        
        public ItemView View => _itemView;
        public ItemType Type => _itemType;
        public int Id => _itemId;
        
        public Item(ItemData itemData)
        {
            _itemView = itemData.ItemView;
            _itemType = itemData.ItemType;
            _itemId = itemData.ItemId;
        }

        public Item()
        {
            _itemView = new();
            _itemType = ItemType.Null;
        }
    }
}