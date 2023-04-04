using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory_Items
{
    public abstract class InventoryView : MonoBehaviour
    {
        public abstract event Action<IReadOnlyCell> CellClicked;

        public abstract void UpdateCells(IReadOnlyList<IReadOnlyCell> cells);
        public abstract void OnCellChanged(IReadOnlyCell oldCell, IReadOnlyCell newCell);
        public abstract void OnCellAdded(IReadOnlyCell cell);
    }
}