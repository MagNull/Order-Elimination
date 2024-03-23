using OrderElimination.Debugging;
using OrderElimination.SavesManagement;
using StartSessionMenu;
using UnityEditor;
using UnityEngine;

namespace OrderElimination.Editor
{
    public static class EditorToolsExtensions
    {
        public static T SelectFirstObjectInScene<T>() where T : Object
        {
            var sceneInstance = 
                Object.FindFirstObjectByType<T>(FindObjectsInactive.Exclude) 
                ?? Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
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

        public static void ShowPath(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath<Object>(path);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        [MenuItem("Tools/Order Elimination/Ability System Analyzer")]
        private static void OpenAnalyzer() => AbilitySystemAnalyzer.OpenWindow();

        [MenuItem("Tools/Order Elimination/Battle/Entity Killer")]
        private static void SelectEntityKiller()
            => EditorToolsExtensions.SelectFirstObjectInScene<EntityKiller>();

        [MenuItem("Tools/Order Elimination/Battle/Entity Factory")]
        private static void SelectEntityFactory()
            => EditorToolsExtensions.SelectFirstObjectInScene<BattleEntitiesFactory>();

        [MenuItem("Tools/Order Elimination/Mediator")]
        private static void SelectMediator()
            => EditorToolsExtensions.SelectFirstObjectInScene<ScenesMediator>();

        [MenuItem("Tools/Order Elimination/Meta Game/Meta Shop")]
        private static void SelectMetaShop()
            => EditorToolsExtensions.SelectFirstObjectInScene<MetaShop>();

        [MenuItem("Tools/Order Elimination/Progress Manager")]
        private static void SelectProgressManager()
            => EditorToolsExtensions.SelectFirstObjectInScene<PlayerProgressManager>();
    }
}
