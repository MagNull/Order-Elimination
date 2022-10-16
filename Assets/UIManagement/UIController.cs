using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIManagement.Debugging;
using UIManagement.Elements;
using UnityEngine;

namespace UIManagement
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private List<IUIPanel> _panels = new List<IUIPanel>();

        public void ShowPanel(PanelType panel)
        {
            _panels.Single(p => p.PanelType == panel).Open();
        }

        public void HidePanel(PanelType panel)
        {
            _panels.Single(p => p.PanelType == panel).Close();
        }

        public void HideAllPanels()
        {
            foreach (var p in _panels)
            {
                p.Close();
            }
        }

        private void Awake()
        {
            _panels = GetComponentsInChildren<IUIPanel>(true).ToList();
            foreach (Transform p in transform)
                p.gameObject.SetActive(true);
            UICommonElementsBuilder.Initialize();
        }

        private void Start()
        {
            HideAllPanels();
            foreach (var p in _panels)
            {
                if (p is IDebuggable debuggable)
                    debuggable.StartDebugging();
            }
        }

        #region temporary debug
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                HideAllPanels();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                HideAllPanels();
                ShowPanel(PanelType.Pause);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                HideAllPanels();
                ShowPanel(PanelType.Order);
            }
        }
        #endregion
    }
}
