using DG.Tweening;
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
        
        public override void Close()
        {
            transform.DOMoveX(UIController.StartPosition, 0.1f).OnComplete(() => base.Close());
        }
    }
}