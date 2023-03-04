using UnityEngine;

namespace UIManagement.Panels
{
    public class ShopPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.Shop;

        public void UpdateShopInfo()
        {
            Debug.Log("Update Shop Info");
        }
    }
}