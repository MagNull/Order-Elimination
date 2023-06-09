using System;

namespace Inventory_Items
{
    public interface IInventoryCellView
    {
        public event Action<IReadOnlyCell> Clicked;
        
        void OnCellChanged(IReadOnlyCell newCell);

        void Enable();
        void Disable();
    }
}