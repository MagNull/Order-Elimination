using System;
using System.Linq;
using UnityEditor;

namespace OrderElimination.Editor
{
    public static class AssetsUtility
    {
        public static T[] GetAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            return AssetDatabase
                .FindAssets($"t: {typeof(T).Name}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(T)) as T)
                .Where(e => e != null)
                .ToArray();
        }

        public static dynamic[] GetAllAssetsByType(Type assetType)
        {
            return AssetDatabase
                .FindAssets($"t: {assetType.Name}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, assetType))
                .Where(e => e != null)
                .ToArray();
        }
    }
}
