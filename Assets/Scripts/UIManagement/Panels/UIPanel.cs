using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIManagement.Debugging;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace UIManagement
{
    /// <summary>
    /// Call Initialize() method before usage.
    /// </summary>
    [DisallowMultipleComponent]
    public abstract class UIPanel : MonoBehaviour, IUIPanel
    {
        protected bool _isInitialized = false;
        [SerializeField] protected TextMeshProUGUI _titleText;
        [SerializeField] protected Button _closingButton;
        [SerializeField] protected bool _isClosingByClickingOutside = true;
        public bool IsClosingByClickingOutside => _isClosingByClickingOutside;

        public event Action<IUIPanel> Opened;
        public event Action<IUIPanel> Closed;

        public string Title => Localization.Current.GetWindowTitleName(PanelType);
        public abstract PanelType PanelType { get; }

        public virtual void Close()
        {
            gameObject.SetActive(false);
            Closed?.Invoke(this);
        }

        public virtual void Open()
        {
            gameObject.SetActive(true);
            Opened?.Invoke(this);
        }
        protected virtual void Initialize()
        {
            _titleText.text = name = Title;
            _closingButton.onClick.RemoveListener(Close);
            _closingButton.onClick.AddListener(Close);
            _isInitialized = true;
        }

        protected void CallOnClosedEvent() => Closed?.Invoke(this);

        protected void CallOnOpenedEvent() => Opened?.Invoke(this);

        protected void OnEnable()
        {
            if (!_isInitialized)
                Initialize();
        }
    }
}
