using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIManagement
{
    public interface IUIPanel
    {
        string Title { get; }
        PanelType PanelType { get; }

        event Action<IUIPanel> Opened;
        event Action<IUIPanel> Closed;

        void Open();

        void Close();
    }
}
