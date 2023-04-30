using ItemsLibrary;
using UnityEngine;
using VContainer;

public class LibraryCheck : MonoBehaviour
{
    private Library _library;
    private Inventory.Inventory _inventory;

    [Inject] 
    public void Configure(Library library, Inventory.Inventory inventory)
    {
        _library = library;
        _inventory = inventory;
    }

    private void Awake()
    {
        _inventory.OnCellAdded += _library.AddItem;
    }
}
