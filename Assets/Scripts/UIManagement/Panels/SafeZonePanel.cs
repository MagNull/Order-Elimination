using DG.Tweening;
using UnityEngine;

namespace UIManagement.Panels
{
    public class SafeZonePanel : UIPanel
    {
        public override PanelType PanelType => PanelType.SafeZone;

        public void UpdateSafeZoneInfo()
        {
            Debug.Log("Update SafeZone Info");
        }
        
        public override void Close()
        {
            transform.DOMoveX(UIController.StartPosition, 0.1f).OnComplete(() => base.Close());
        }
    }
}