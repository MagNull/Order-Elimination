using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using UnityEngine;
using UnityEngine.UI;

namespace GameInventory.Views
{
    public class CharacterInventoryView : InventoryView
    {
        [SerializeField]
        private Dictionary<ItemType, Image> _cellViewByItemType = new();

        public override event Action<IReadOnlyCell> CellClicked;

        //TODO: Refactor
        public override void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            foreach (var image in _cellViewByItemType)
            {
                image.Value.enabled = false;
            }

            if (!cells.Any()) 
                return;
            
            foreach (var cell in cells)
            {
                OnCellAdded(cell);
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