using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIManagement.Debugging;
using UIManagement.Elements;
using UnityEngine;

namespace UIManagement
{
    [ExecuteAlways]
    public class UIController : MonoBehaviour
    {
        [SerializeField] private List<IUIPanel> _panels = new List<IUIPanel>();

        [Button, DisableInEditorMode]
        public void ShowPanelIsolated(PanelType panel)
        {
            CloseAllPanels();
            OpenPanel(panel);
        }

        public IUIPanel OpenPanel(PanelType panel)
        {
            var panelToOpen = _panels.Single(p => p.PanelType == panel);
            panelToOpen.Open();
            return panelToOpen;
        }

        public void ClosePanel(PanelType panel)
        {
            _panels.Single(p => p.PanelType == panel).Close();
        }

        public void CloseAllPanels()
        {
            foreach (var p in _panels)
            {
                p.Close();
            }
        }

        private void Awake()
        {
            foreach (Transform p in transform)
                p.gameObject.SetActive(true);
            _panels = GetComponentsInChildren<IUIPanel>(true).ToList();
        }

        //private void Start()
        //{
        //    HideAllPanels();
        //    foreach (var p in _panels)
        //    {
        //        if (p is IDebuggable debuggable)
        //            debuggable.StartDebugging();
        //    }
        //}
    }
}
