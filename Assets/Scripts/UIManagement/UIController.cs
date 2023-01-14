using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIManagement.Debugging;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIController : MonoBehaviour
    {
        private List<IUIPanel> _panels = new List<IUIPanel>();
        private List<IUIPanel> _openedPanelsStack = new List<IUIPanel>();
        [SerializeField]
        private Button _closingArea;
        [SerializeField]
        private float _windowOpeningTime = 0.3f;
        [SerializeField]
        private Ease _windowOpeningEase = Ease.Flash;
        public static UIController SceneInstance { get; private set; }

        [Button, DisableInEditorMode]
        public IUIPanel OpenPanel(PanelType panel)
        {
            var panelToOpen = _panels.Single(p => p.PanelType == panel);
            _closingArea.gameObject.SetActive(true);
            panelToOpen.Closed += OnPanelClosed;

            var panelTransform = ((UIPanel)panelToOpen).transform;
            panelTransform.SetSiblingIndex(_panels.Count + 1);
            _openedPanelsStack.Add(panelToOpen);

            panelToOpen.Open();
            panelTransform.localScale = Vector3.one * 0.1f;
            panelTransform.DOScale(1, _windowOpeningTime).SetEase(_windowOpeningEase);
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
            SceneInstance = this;
            //foreach (Transform p in transform)
            //    p.gameObject.SetActive(true);
            _panels = GetComponentsInChildren<IUIPanel>(true).ToList();
            _closingArea.onClick.RemoveListener(CloseOntopPanel);
            _closingArea.onClick.AddListener(CloseOntopPanel);
        }

        private void CloseOntopPanel()
        {
            if (!_openedPanelsStack[_openedPanelsStack.Count - 1].IsClosingByClickingOutside)
                return;
            _openedPanelsStack[_openedPanelsStack.Count - 1].Close();
        }

        private void OnPanelClosed(IUIPanel panel)
        {
            panel.Closed -= OnPanelClosed;
            _openedPanelsStack.Remove(panel);
            if (_openedPanelsStack.Count == 0)
                _closingArea.gameObject.SetActive(false);
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
