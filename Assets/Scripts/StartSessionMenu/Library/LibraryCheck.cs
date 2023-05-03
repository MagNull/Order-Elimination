using ItemsLibrary;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using UnityEngine;
using VContainer;

public class LibraryCheck : MonoBehaviour
{
    private Library _library;
    private Inventory_Items.Inventory _inventory;
    [SerializeField] private PanelGenerator _generator;
    [SerializeField] private Squad _squad;

    [Inject] 
    public void Configure(Library library, Inventory_Items.Inventory inventory)
    {
        _library = library;
        _inventory = inventory;
        _inventory.OnCellAdded += _library.AddItem;
        _generator.OnInitializedPanels += SetShopPanelReference;
    }

    private void SetShopPanelReference()
    {
        _generator.GetPanelByPointInfo(PointType.Shop).GetComponent<ShopPanel>().OnBuyItems += _library.AddItem;
    }
}
