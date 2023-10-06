using System;
using System.Collections.Generic;
using GameInventory.Items;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;

namespace GameInventory.Views
{
    public class CharacterInventoryView : InventoryView
    {
        [SerializeField]
        private Image[] _cellImages;

        public override event Action<IReadOnlyCell> CellClicked;
        private readonly List<(Image SlotImage, IReadOnlyCell Cell)> _imageToCell = new();

        private void Awake() => FillInventoryCells();

        public override void UpdateCells(IReadOnlyList<IReadOnlyCell> cells)
        {
            if (_imageToCell.Count == 0)
                FillInventoryCells();

            for (var i = 0; i < cells.Count; i++)
            {
                _imageToCell[i] = (_cellImages[i], cells[i]);
                if (_imageToCell[i].Cell.Item is not EmptyItem)
                {
                    _imageToCell[i].SlotImage.sprite = _imageToCell[i].Item2.Item.Data.View.Icon;
                    _imageToCell[i].SlotImage.enabled = enabled;
                }
                else
                {
                    _imageToCell[i].SlotImage.enabled = false;
                }
            }
        }

        public override void OnCellChanged(IReadOnlyCell cell)
        {
            for (var i = 0; i < _imageToCell.Count; i++)
            {
                if (_imageToCell[i].Cell != cell)
                    continue;

                if (cell.Item is EmptyItem)
                    _imageToCell[i].SlotImage.enabled = false;
                else
                {
                    _imageToCell[i].SlotImage.enabled = true;
                    _imageToCell[i].SlotImage.sprite = cell.Item.Data.View.Icon;
                }

                return;
            }

            Logging.LogException(new Exception("Cell not found"));
        }

        private void FillInventoryCells()
        {
            foreach (var image in _cellImages)
            {
                _imageToCell.Add((image, null));
                image.enabled = false;
            }
        }
    }
}