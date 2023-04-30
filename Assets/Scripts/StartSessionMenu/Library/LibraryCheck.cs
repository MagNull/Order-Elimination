using ItemsLibrary;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using UnityEngine;
using VContainer;

public class LibraryCheck : MonoBehaviour
{
    private Library _library;
    private Inventory_Items.Inventory _inventory;
    [SerializeField] private PanelGenerator _generator;

    [Inject] 
    public void Configure(Library library, Inventory_Items.Inventory inventory)
    {
        _library = library;
        _inventory = inventory;
    }

    private void Start()
    {
        _inventory.OnCellAdded += _library.AddItem;
        _generator.OnInitializedPanels += SetShopPanelReference;
    }

    private void SetShopPanelReference()
    {
        _generator.GetPanelByPointInfo(PointType.Shop).GetComponent<ShopPanel>().OnBuyItems += _library.AddItem;
    }
}
