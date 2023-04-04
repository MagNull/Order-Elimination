namespace Inventory_Items
{
    public class Item
    {
        private ItemView _itemView;
        private ItemType _itemType;
        
        public ItemView View => _itemView;
        public ItemType Type => _itemType;
        
        public Item(ItemData itemData)
        {
            _itemView = itemData.ItemView;
            _itemType = itemData.ItemType;
        }

    }
}