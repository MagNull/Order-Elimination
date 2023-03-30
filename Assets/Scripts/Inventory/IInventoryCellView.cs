namespace Inventory
{
    public interface IInventoryCellView
    {
        void OnCellChanged(IReadOnlyCell newCell);

        void Enable();
        void Disable();
    }
}