﻿using System;
using Sirenix.OdinInspector;

namespace Inventory_Items
{
    [Serializable]
    public class Cell : IReadOnlyCell
    {
        [ShowInInspector]
        private readonly Item _item;
        private readonly int _itemQuantity;

        public Item Item => _item;

        public int ItemQuantity => _itemQuantity;

        public Cell(Item item = null, int itemQuantity = 0)
        {
            if (itemQuantity < 0)
                throw new ArgumentException("Item quantity can't be less than zero");
            
            _itemQuantity = itemQuantity;
            
            _item = itemQuantity == 0 ? new Item() : item;
        }
    }
    
    public interface IReadOnlyCell
    {
        Item Item { get; }
        int ItemQuantity { get; }
    }
}