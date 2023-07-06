using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
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
            if (cells.Any())
            {
                foreach (var cell in cells)
                {
                    OnCellAdded(cell);
                }
            }
            else
            {
                foreach (var image in _cellViewByItemType)
                {
                    image.Value.enabled = false;
                }
            }
        }

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            if (!_cellViewByItemType.ContainsKey(cell.Item.Type)) 
                return;
            _cellViewByItemType[cell.Item.Type].sprite = cell.Item.View.Icon;
            _cellViewByItemType[cell.Item.Type].enabled = true;
        }

        public override void OnCellRemoved(IReadOnlyCell cell)
        {
            _cellViewByItemType[cell.Item.Type].enabled = false;
        }
    }
}