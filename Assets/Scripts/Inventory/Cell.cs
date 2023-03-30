using System;

namespace Inventory
{
    public class Cell : IReadOnlyCell
    {
        private readonly Item _item;
        private readonly int _itemQuantity;

        public Item Item => _item;

        public int ItemQuantity => _itemQuantity;

        public Cell(Item item = null, int itemQuantity = 0)
        {
            if (itemQuantity < 0)
                throw new ArgumentException("Item quantity can't be less than zero");
            
            _itemQuantity = itemQuantity;
            
            _item = itemQuantity == 0 ? null : item;
        }
    }
    
    public interface IReadOnlyCell
    {
        Item Item { get; }
        int ItemQuantity { get; }
    }
}