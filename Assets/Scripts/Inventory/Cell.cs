﻿using System;
using Sirenix.OdinInspector;

namespace Inventory_Items
{
    [Serializable]
    public class Cell : IReadOnlyCell
    {
        [ShowInInspector]
        private readonly Item _item;

        public Item Item => _item;


        public Cell(Item item)
        {
            _item = item;
        }
    }

    public interface IReadOnlyCell
    {
        Item Item { get; }
    }
}