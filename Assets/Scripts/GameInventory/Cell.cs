using System;
using GameInventory.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameInventory
{
    [Serializable]
    public class Cell : IReadOnlyCell
    {
        [SerializeReference]
        private Item _item;

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