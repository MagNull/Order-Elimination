namespace Inventory
{
    public class Item
    {
        private ItemView _itemView;
        private ItemType _itemType;
        private int _itemIndex;
        
        public ItemView View => _itemView;
        public ItemType Type => _itemType;

        public int Index => _itemIndex;
        
        public Item(ItemData itemData)
        {
            _itemView = itemData.ItemView;
            _itemType = itemData.ItemType;
            _itemIndex = itemData.ItemIndex;
        }

    }
}