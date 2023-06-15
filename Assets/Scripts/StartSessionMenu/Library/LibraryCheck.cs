using ItemsLibrary;
using RoguelikeMap.Panels;
using RoguelikeMap.Points;
using RoguelikeMap.SquadInfo;
using RoguelikeMap.UI.PointPanels;
using UnityEngine;
using VContainer;

public class LibraryCheck : MonoBehaviour
{
    private Library _library;
    private Inventory_Items.Inventory _inventory;
    private ShopPanel _shopPanel;
    [SerializeField] private Squad _squad;

    [Inject] 
    public void Configure(Library library, Inventory_Items.Inventory inventory, PanelManager panelManager)
    {
        _library = library;
        _inventory = inventory;
        _inventory.OnCellAdded += _library.AddItem;
        var shopPanel = (ShopPanel) panelManager.GetPanelByPointInfo(PointType.Shop);
        shopPanel.OnBuyItems += _library.AddItem;
    }
}
