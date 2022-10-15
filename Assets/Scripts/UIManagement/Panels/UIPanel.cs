using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIManagement.Debugging;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement
{
    public class UIPanel : MonoBehaviour, IUIPanel
    {
        private GameObject _gameObject;
        [SerializeField] protected string _windowTitle;
        [SerializeField] protected TextMeshProUGUI _titleText;

        public event Action<IUIPanel> Opened;
        public event Action<IUIPanel> Closed;

        public string Title => _windowTitle;
        public PanelType PanelType { get; }

        public virtual void Close()
        {
            _gameObject.SetActive(false);
            Closed?.Invoke(this);
        }

        public virtual void Open()
        {
            _gameObject.SetActive(true);
            Opened?.Invoke(this);
        }
        private void Awake()
        {
            _gameObject = gameObject;
        }
    }
}
