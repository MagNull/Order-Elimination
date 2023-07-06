using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Inventory_Items
{
    public abstract class InventoryView : SerializedMonoBehaviour
    {
        public abstract event Action<IReadOnlyCell> CellClicked;

        public abstract void UpdateCells(IReadOnlyList<IReadOnlyCell> cells);
        public abstract void OnCellAdded(IReadOnlyCell cell);
        public abstract void OnCellRemoved(IReadOnlyCell cell);
    }
}