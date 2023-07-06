using System;

namespace Inventory_Items
{
    public interface IInventoryCellView
    {
        public event Action<IReadOnlyCell> Clicked;
        public void Init(IReadOnlyCell cell);
        void Enable();
        void Disable();
    }
}