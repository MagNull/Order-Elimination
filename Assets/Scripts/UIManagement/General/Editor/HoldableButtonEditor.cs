using UIManagement.Elements;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(HoldableButton))]
public class HoldableButtonEditor : ButtonEditor
{
    public override void OnInspectorGUI()
    {
        var holdableButton = (HoldableButton)target;
        EditorUtility.SetDirty(holdableButton);
        holdableButton.ClickAvailable = EditorGUILayout.Toggle("Click available", holdableButton.ClickAvailable);
        holdableButton.HoldAvailable = EditorGUILayout.Toggle("Hold available", holdableButton.HoldAvailable);
        if (holdableButton.HoldAvailable)
        {
            holdableButton.MillisecondsToHold = EditorGUILayout.IntSlider(
                "Holding time (ms)", holdableButton.MillisecondsToHold, 10, 10000);
        }
        holdableButton.ClickUnavalableTint = EditorGUILayout.ColorField("Click unavalable tint", holdableButton.ClickUnavalableTint);
        holdableButton.UpdateVisuals();
        base.OnInspectorGUI();
    }
}
