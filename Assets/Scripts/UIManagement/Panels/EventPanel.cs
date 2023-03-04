using DG.Tweening;
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

        public override void Close()
        {
            transform.DOMoveX(UIController.StartPosition, 0.1f).OnStepComplete(() => base.Close());
        }
    }
}