//using System.Collections;
//using System.Collections.Generic;
//using UIManagement.Elements;
//using UIManagement.trashToRemove_Mockups;
//using UnityEditor;
//using UnityEngine;

//[CustomEditor(typeof(CharacterList))]
//public class Editor_CharacterList : Editor
//{
//    private int _id = 0;
//    private int _count = 0;

//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();
//        CharacterList list = (CharacterList)target;
//        list.HasExperienceRecieved = EditorGUILayout.Toggle("Has experience recieved", list.HasExperienceRecieved);
//        list.HasMaintenanceCost = EditorGUILayout.Toggle("Has maintenance cost", list.HasMaintenanceCost);
//        list.HasParameters = EditorGUILayout.Toggle("Has parameters", list.HasParameters);
//        EditorGUILayout.BeginHorizontal();
//        _count = EditorGUILayout.IntField("Elements count", list.CharactersCount);
//        if (_count < 0)
//            _count = 0;
//        if (_count != list.CharactersCount)
//        {
//            UpdateListCount(list, _count);
//        }
//        if (GUILayout.Button("Add"))
//        {
//            UpdateListCount(list, list.CharactersCount + 1);
//        }
//        if (GUILayout.Button("Remove"))
//        {
//            UpdateListCount(list, Mathf.Max(list.CharactersCount - 1, 0));
//        }
//        EditorGUILayout.EndHorizontal();
//        if (list.CharactersCount > 0)
//        {
//            EditorGUILayout.BeginHorizontal();
//            _id = EditorGUILayout.IntSlider("Index", _id, 0, list.CharactersCount - 1);
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

//    private void UpdateListCount(CharacterList list, int expectedCount)
//    {
//        if (expectedCount < 0)
//            throw new System.ArgumentException();
//        while (list.CharactersCount != expectedCount)
//        {
//            if (list.CharactersCount < expectedCount)
//            {
//                list.Add(new BattleCharacterView());
//            }
//            else if (list.CharactersCount > expectedCount)
//            {
//                list.RemoveAt(list.CharactersCount - 1);
//            }
//        }
//    }
//}
