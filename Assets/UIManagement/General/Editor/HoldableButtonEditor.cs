using CharacterAbility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

[CustomEditor(typeof(HoldableButton))]
public class HoldableButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        var holdableButton = (HoldableButton)target;
        holdableButton.MillisecondsToHold = EditorGUILayout.IntSlider(
            "Holding time (ms)", holdableButton.MillisecondsToHold, 10, 10000);
        base.OnInspectorGUI();
    }
}
