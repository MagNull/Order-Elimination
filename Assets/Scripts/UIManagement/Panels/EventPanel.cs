using UnityEngine;

namespace UIManagement.Panels
{
    public class EventPanel : UIPanel
    {
        public override PanelType PanelType => PanelType.Event;
        
        public void UpdateEventInfo()
        {
            Debug.Log("Update Event Info");
        }
    }
}