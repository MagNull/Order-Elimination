using System.Collections;
using System.Collections.Generic;
using UIManagement.Elements;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PageSwitcher))]
public class Editor_PageSwitcher : Editor
{
    private int _id = 0;
    private int _count = 0;
    private bool _unsafeMode = false;
    // = new GameObject("New Page", typeof(RectTransform));

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PageSwitcher pageSwitcher = (PageSwitcher)target;
        EditorGUILayout.BeginHorizontal();
        _count = EditorGUILayout.IntField("Elements count", pageSwitcher.PageCount);
        if (_count < 0)
            _count = 0;
        if (_count != pageSwitcher.PageCount)
        {
            UpdateListCount(pageSwitcher, _count);
        }
        if (GUILayout.Button("Add"))
        {
            UpdateListCount(pageSwitcher, pageSwitcher.PageCount + 1);
        }
        if (GUILayout.Button("Remove"))
        {
            UpdateListCount(pageSwitcher, Mathf.Max(pageSwitcher.PageCount - 1, 0));
        }
        EditorGUILayout.EndHorizontal();
        if (pageSwitcher.PageCount > 0)
        {
            EditorGUILayout.BeginHorizontal();
            _id = EditorGUILayout.IntSlider("Index", _id, 0, pageSwitcher.PageCount - 1);
            if (GUILayout.Button("Remove element"))
            {
                pageSwitcher.RemoveAt(_id);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show page"))
            {
                pageSwitcher.ShowPage(_id);
            }
            if (GUILayout.Button("Disable page"))
            {
                pageSwitcher.DisablePage(_id);
            }
            if (GUILayout.Button("Enable page"))
            {
                pageSwitcher.EnablePage(_id);
            }
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button("Clear"))
            {
                pageSwitcher.Clear();
            }
        }
        if (pageSwitcher.HasForeignChildren && GUILayout.Button("Destroy foreign children"))
        {
            pageSwitcher.DestroyAllChildrenNotInList();
        }
        if (pageSwitcher.HasForeignChildren && GUILayout.Button("Attach foreign children"))
        {
            pageSwitcher.AttachForeignChildren();
        }
        _unsafeMode = GUILayout.Toggle(_unsafeMode, "«I'm ready to break it» mode");
        if (pageSwitcher.PageCount > 0 && _unsafeMode && GUILayout.Button("Colorize Test"))
        {
            pageSwitcher.ColorizeTest();
        }
    }

    private void UpdateListCount(PageSwitcher pageSwitcher, int expectedCount)
    {
        if (expectedCount < 0)
            throw new System.ArgumentException();
        while (pageSwitcher.PageCount != expectedCount)
        {
            if (pageSwitcher.PageCount < expectedCount)
            {
                var pageName = $"Page {pageSwitcher.PageCount}";
                pageSwitcher.AddPage(pageName, null);
            }
            else if (pageSwitcher.PageCount > expectedCount)
            {
                pageSwitcher.RemoveAt(pageSwitcher.PageCount - 1);
            }
        }
    }
}
