using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UIManagement.Debugging
{
    public interface IDebuggablePanel<TPanel> : IDebuggable, IUIPanel
        where TPanel:MonoBehaviour, IUIPanel
    {

    }

    public static class IDebuggablePanelExtensions
    {
        public static void ButtonPressedDebug<TPanel>(this IDebuggablePanel<TPanel> panel, string buttonName)
            where TPanel : MonoBehaviour, IUIPanel
        {
            Debug.Log($"{buttonName} pressed on {nameof(panel.PanelType)}:{panel.PanelType} \"{(panel as TPanel).name}\"");
        }
    }
}
