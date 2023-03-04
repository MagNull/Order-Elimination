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
    }
}