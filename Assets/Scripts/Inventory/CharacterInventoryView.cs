using System;
using System.Collections.Generic;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_Items
{
    public class CharacterInventoryView : InventoryView
    {
        [SerializeField]
        private Dictionary<ItemType, Image> _cellViewByItemType = new();

        public override event Action<IReadOnlyCell> CellClicked;
        public override void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            foreach (var cell in cells)
            {
                if (_cellViewByItemType.ContainsKey(cell.Item.Type))
                    _cellViewByItemType[cell.Item.Type].sprite = cell.Item.View.Icon;
            }
        }

        public override void OnCellChanged(IReadOnlyCell oldCell, IReadOnlyCell newCell)
        {
            _cellViewByItemType[oldCell.Item.Type].sprite = newCell.Item.View.Icon;
        }

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            if (_cellViewByItemType.ContainsKey(cell.Item.Type))
                _cellViewByItemType[cell.Item.Type].sprite = cell.Item.View.Icon;
        }
    }
}