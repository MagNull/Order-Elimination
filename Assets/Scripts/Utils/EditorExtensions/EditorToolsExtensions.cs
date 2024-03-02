using UnityEditor;
using UnityEngine;

namespace OrderElimination.Editor
{
    public static class EditorToolsExtensions
    {
        public static T SelectFirstObjectInScene<T>() where T : Object
        {
            var sceneInstance = Object.FindFirstObjectByType<T>();
            if (sceneInstance == null)
            {
                Logging.LogError($"There are no {typeof(T).Name} objects in the scene");
                return null;
            }
#if UNITY_EDITOR
            Selection.activeObject = sceneInstance;
            EditorGUIUtility.PingObject(sceneInstance);
#endif
            return sceneInstance;
        }
    }
}
