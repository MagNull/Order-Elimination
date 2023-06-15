//using System.Collections;
//using System.Collections.Generic;
//using UIManagement.Elements;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(IconTextValueList))]
//public class Editor_IconTextValueList : Editor
//{
//    private int _id = 0;
//    private int _count = 0;

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        IconTextValueList list = (IconTextValueList)target;
//        //list.DestroyAllChildrenNotInList();
//        list.HasIcons = EditorGUILayout.Toggle("Has icons", list.HasIcons);
//        list.HasTexts = EditorGUILayout.Toggle("Has texts", list.HasTexts);
//        list.HasValues = EditorGUILayout.Toggle("Has values", list.HasValues);
//        list.IconSize = EditorGUILayout.FloatField("Icon size", list.IconSize);
//        EditorGUILayout.BeginHorizontal();
//        _count = EditorGUILayout.IntField("Elements count", list.Count);
//        if (_count < 0)
//            _count = 0;
//        if (_count != list.Count)
//        {
//            UpdateListCount(list, _count);
//        }
//        if (GUILayout.Button("Add"))
//        {
//            UpdateListCount(list, list.Count + 1);
//        }
//        if (GUILayout.Button("Remove"))
//        {
//            UpdateListCount(list, Mathf.Max(list.Count - 1, 0));
//        }
//        EditorGUILayout.EndHorizontal();
//        if (list.Count > 0)
//        {
//            EditorGUILayout.BeginHorizontal();
//            _id = EditorGUILayout.IntSlider("Index", _id, 0, list.Count - 1);
//            if (GUILayout.Button("Remove element"))
//            {
//                list.RemoveAt(_id);
//            }
//            EditorGUILayout.EndHorizontal();
//            if (GUILayout.Button("Clear"))
//            {
//                list.Clear();
//            }
//        }
//        if (list.HasForeignChildren && GUILayout.Button("Destroy foreign children"))
//        {
//            list.DestroyAllChildrenNotInList();
//        }
//    }

//    private void UpdateListCount(IconTextValueList list, int expectedCount)
//    {
//        if (expectedCount < 0)
//            Logging.LogException( new System.ArgumentException();
//        while (list.Count != expectedCount)
//        {
//            if (list.Count < expectedCount)
//            {
//                list.Add();
//            }
//            else if (list.Count > expectedCount)
//            {
//                list.RemoveAt(list.Count - 1);
//            }
//        }
//    }
//}
