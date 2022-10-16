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
    public abstract class UIPanel : MonoBehaviour, IUIPanel
    {
        protected GameObject _gameObject;
        [SerializeField] protected TextMeshProUGUI _titleText;

        public event Action<IUIPanel> Opened;
        public event Action<IUIPanel> Closed;

        public string Title => Localization.Current.GetWindowTitleName(PanelType);
        public abstract PanelType PanelType { get; }

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
        protected virtual void Initialize()
        {
            _gameObject = gameObject;
            _titleText.text = Title;
            name = Title;
        }

        protected void CallOnClosedEvent() => Closed?.Invoke(this);

        protected void CallOnOpenedEvent() => Opened?.Invoke(this);
    }
}
