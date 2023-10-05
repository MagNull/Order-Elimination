using UnityEditor;
using UnityEngine;

namespace OrderElimination.Editor
{
    public static class InspectorGUIExtensions
    {
        public static void DrawLabel(Rect rect, string text, Color color)
        {
#if UNITY_EDITOR
            var prevColor = GUI.contentColor;
            var style = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
            GUI.contentColor = color;
            GUI.Label(rect, text, style);
            GUI.contentColor = prevColor;
#endif
        }
    }
}
