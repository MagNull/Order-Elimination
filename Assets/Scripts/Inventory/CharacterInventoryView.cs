using System;
using System.Collections.Generic;
using System.Linq;
using Inventory;
using OrderElimination;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory_Items
{
    public class CharacterInventoryView : InventoryView
    {
        [SerializeField]
        private Dictionary<ItemType, Image> _cellViewByItemType = new();

        [SerializeField]
        private Sprite _emptyCellSprite;

        public override event Action<IReadOnlyCell> CellClicked;

        public override void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            if (cells.Any())
            {
                foreach (var cell in cells)
                {
                    if (_cellViewByItemType.ContainsKey(cell.Item.Type))
                    {
                        _cellViewByItemType[cell.Item.Type].sprite = cell.Item.View.Icon;
                    }
                }
            }
            else
            {
                foreach (var image in _cellViewByItemType)
                {
                    image.Value.sprite = _emptyCellSprite;
                }
            }
        }

        public override void OnCellChanged(IReadOnlyCell oldCell, IReadOnlyCell newCell)
        {
            _cellViewByItemType[oldCell.Item.Type].sprite = newCell.Item.View.Icon;
        }

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            Logging.Log("OnCellAdded", context: this);
            if (_cellViewByItemType.ContainsKey(cell.Item.Type))
                _cellViewByItemType[cell.Item.Type].sprite = cell.Item.View.Icon;
        }

        public override void OnCellRemoved(IReadOnlyCell cell)
        {
            _cellViewByItemType[cell.Item.Type].sprite = _emptyCellSprite;
        }
    }
}