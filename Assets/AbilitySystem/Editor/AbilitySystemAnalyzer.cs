using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AbilitySystemAnalyzer : OdinMenuEditorWindow
{
    #region SupportTypes
    public enum AbilitySystemAssetType
    {
        ActiveAbility,
        PassiveAbility,
        Effect
    }

    private class AbilitySystemAnalyzerInfo
    {
        private enum SearchForOption
        {
            TypeReference,
            InstanceReference
        }

        private ActiveAbilityBuilder[] _projectActiveAbilities;
        private PassiveAbilityBuilder[] _projectPassiveAbilities;
        private EffectDataPreset[] _projectEffects;
        private CharacterTemplate[] _projectCharacters;
        private StructureTemplate[] _projectStructures;

        [TitleGroup("Statistics")]
        [GUIColor("@Color.yellow")]
        [ShowInInspector]
        public int TotalAbilitySystemAssets 
            => TotalActiveAbilities 
            + TotalPassiveAbilities 
            + TotalEffects 
            + TotalCharacters 
            + TotalStructures;

        [TitleGroup("Statistics")]
        [ShowInInspector]
        public int TotalActiveAbilities => _projectActiveAbilities.Length;
        
        [TitleGroup("Statistics")]
        [ShowInInspector]
        public int TotalPassiveAbilities => _projectPassiveAbilities.Length;

        [TitleGroup("Statistics")]
        [ShowInInspector]
        public int TotalEffects => _projectEffects.Length;

        [TitleGroup("Statistics")]
        [ShowInInspector]
        public int TotalCharacters => _projectCharacters.Length;

        [TitleGroup("Statistics")]
        [ShowInInspector]
        public int TotalStructures => _projectStructures.Length;

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ValidateInput("@false", "Only searches for serialized parameters")]
        [ShowInInspector]
        private SearchForOption SearchFor { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowIf("@SearchFor == SearchForOption.InstanceReference")]
        [ShowInInspector]
        public object SeekingInstance { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowIf("@SearchFor == SearchForOption.TypeReference")]
        [ShowInInspector]
        public Type SeekingType { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowInInspector]
        public bool DirectReferenceOnly { get; set; } = true;

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowInInspector]
        public bool IncludeAnimations { get; set; } = true;

        [VerticalGroup("Dependencies Search/Results")]
        [ShowIf("@" + nameof(SearchResult) + " != null")]
        //[HideLabel, Title("Auto Select Asset")]
        [ShowInInspector]
        public bool AutoSelectAsset { get; set; } = false;

        [VerticalGroup("Dependencies Search/Results")]
        [ShowIf("@" + nameof(SearchResult) + " != null && " + nameof(SearchResult) + ".MenuItems.Count > 0")]
        [HideLabel, OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector]
        private OdinMenuTree SearchResult { get; set; }

        public AbilitySystemAnalyzerInfo()
        {
            _projectActiveAbilities = AssetDatabase
                .FindAssets($"t: {nameof(ActiveAbilityBuilder)}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(ActiveAbilityBuilder)) as ActiveAbilityBuilder)
                .Where(e => e != null)
                .ToArray();
            _projectPassiveAbilities = AssetDatabase
                .FindAssets($"t: {nameof(PassiveAbilityBuilder)}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(PassiveAbilityBuilder)) as PassiveAbilityBuilder)
                .Where(e => e != null)
                .ToArray();
            _projectEffects = AssetDatabase
                .FindAssets($"t: {nameof(EffectDataPreset)}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(EffectDataPreset)) as EffectDataPreset)
                .Where(e => e != null)
                .ToArray();
            _projectCharacters = AssetDatabase
                .FindAssets($"t: {nameof(CharacterTemplate)}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(CharacterTemplate)) as CharacterTemplate)
                .Where(e => e != null)
                .ToArray();
            _projectStructures = AssetDatabase
                .FindAssets($"t: {nameof(StructureTemplate)}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(StructureTemplate)) as StructureTemplate)
                .Where(e => e != null)
                .ToArray();
        }

        [VerticalGroup("Dependencies Search/Parameters")]
        [PropertyTooltip(
            "Cascading search for serialized fields and properties with assigned value of a seeking type.")]
        [Button, GUIColor("@Color.cyan")]
        public void FindAssignedDependencies()
        {
            if (SearchFor == SearchForOption.TypeReference && SeekingType == null)
            {
                Logging.LogError("Wrong search type.");
                return;
            }
            if (SearchFor == SearchForOption.InstanceReference && SeekingInstance == null)
            {
                Logging.LogError("Wrong seeking instance.");
                return;
            }
            ScriptableObject[] assets = _projectActiveAbilities
                .AsEnumerable<ScriptableObject>()
                .Concat(_projectPassiveAbilities)
                .Concat(_projectEffects)
                .Concat(_projectCharacters)
                .Concat(_projectStructures)
                .ToArray();
            var foundAssets = new List<ScriptableObject>();
            var assetsWithExceptions = new Dictionary<ScriptableObject, Exception>();

            if (SearchFor == SearchForOption.TypeReference)
            {
                foreach (var asset in assets)
                {
                    bool success = false;
                    Exception exception = null;
                    try
                    {
                        if (HasChildrenOfType(asset, SeekingType, new()))
                            success = true;
                    }
                    catch (Exception e)
                    {
                        Logging.LogError($"Exception on {asset.name}");
                        throw e;
                        exception = e;
                    }
                    if (success)
                        foundAssets.Add(asset);
                    if (exception != null)
                        assetsWithExceptions.Add(asset, exception);
                }
            }
            if (SearchFor == SearchForOption.InstanceReference)
            {
                foreach (var asset in assets)
                {
                    bool success = false;
                    Exception exception = null;
                    try
                    {
                        if (HasInstanceInChildren(asset, SeekingInstance, new()))
                            success = true;
                    }
                    catch (Exception e)
                    {
                        Logging.LogError($"Exception on {asset.name}");
                        throw e;
                        exception = e;
                    }
                    if (success)
                        foundAssets.Add(asset);
                    if (exception != null)
                        assetsWithExceptions.Add(asset, exception);
                }
            }

            if (SearchResult != null)
            {
                SearchResult.Selection.SelectionConfirmed -= OnSelectionConfirmed;
                SearchResult.Selection.SelectionChanged -= OnSelectionChanged;
            }
            var searchTree = new OdinMenuTree(false);
            searchTree.Selection.SelectionConfirmed += OnSelectionConfirmed;
            searchTree.Selection.SelectionChanged += OnSelectionChanged;
            SearchResult = searchTree;
            AddToSearchResultDisplay($"Found Assets ({foundAssets.Count})", foundAssets);
            AddToSearchResultDisplay($"Assets with Exceptions ({assetsWithExceptions.Count})", assetsWithExceptions.Keys);
            if (assetsWithExceptions.Count > 0)
            {
                Logging.LogError(
                    $"Assets with exceptions ({assetsWithExceptions.Count}):\n\t" % Colorize.Red
                    + string.Join("\n\t", assetsWithExceptions.Select(a => $"Asset with exception: \"{a.Key.name}\": {a.Value.GetExceptionName()}"))
                    + "\n");
            }
        }

        private bool HasChildrenOfType(
            object instance, Type seekingType, HashSet<ScriptableObject> openedAssets)
        {
            if (instance.IsAbilitySystemAsset())
                openedAssets.Add((ScriptableObject)instance);
            var members = ReflectionExtensions.GetSerializedMembers(instance)
                .Where(e => !e.IsAbilitySystemAsset() || !DirectReferenceOnly && !openedAssets.Contains(e))
                .Where(e => e is not IAbilityAnimation || IncludeAnimations);
            foreach (var value in members)
            {
                var valueType = value.GetType();
                var isSeekingType = seekingType.IsAssignableFrom(valueType);
                if (isSeekingType)
                    return true;//<- field/property of seeking type found
                if (value is not ICollection)
                {
                    if (HasChildrenOfType(value, seekingType, openedAssets))
                        return true;
                }
            }
            return false;
        }

        private bool HasInstanceInChildren(
            object obj, object seekingInstance, HashSet<ScriptableObject> openedAssets)
        {
            if (obj.IsAbilitySystemAsset())
                openedAssets.Add((ScriptableObject)obj);
            var members = ReflectionExtensions.GetSerializedMembers(obj)
                .Where(e => !e.IsAbilitySystemAsset() || !DirectReferenceOnly && !openedAssets.Contains(e))
                .Where(e => e is not IAbilityAnimation || IncludeAnimations);
            foreach (var value in members)
            {
                if (value == seekingInstance)
                    return true;
                if (value is not ICollection)
                {
                    if (HasInstanceInChildren(value, seekingInstance, openedAssets))
                        return true;
                }
            }
            return false;
        }

        private void AddToSearchResultDisplay(string categoryName, IEnumerable<UnityEngine.Object> assets)
        {
            foreach (var foundAsset in assets)
            {
                if (foundAsset is ActiveAbilityBuilder activeAbility)
                {
                    SearchResult.Add($"{categoryName}/Active Abilities/{foundAsset.name}", foundAsset, activeAbility.Icon);
                }
                else if (foundAsset is PassiveAbilityBuilder passiveAbility)
                {
                    SearchResult.Add($"{categoryName}/Passive Abilities/{foundAsset.name}", foundAsset, passiveAbility.Icon);
                }
                else if (foundAsset is EffectDataPreset effect)
                {
                    SearchResult.Add($"{categoryName}/Effects/{foundAsset.name}", foundAsset, effect.View.Icon);
                }
                else if (foundAsset is CharacterTemplate character)
                {
                    SearchResult.Add($"{categoryName}/Characters/{foundAsset.name}", foundAsset, character.BattleIcon);
                }
                else if (foundAsset is StructureTemplate structure)
                {
                    SearchResult.Add($"{categoryName}/Structures/{foundAsset.name}", foundAsset, structure.BattleIcon);
                }
                else
                {
                    SearchResult.Add($"{categoryName}/Unknown/{foundAsset.name}", foundAsset);
                }
            }
            var menuItem = SearchResult.GetMenuItem(categoryName);
            if (menuItem != null)
                menuItem.MenuTree.SortMenuItemsByName();
        }

        private void OnSelectionChanged(SelectionChangedType changeType)
        {
            if (changeType == SelectionChangedType.ItemAdded && AutoSelectAsset)
            {
                if (SearchResult.Selection.SelectedValue != null
                    && SearchResult.Selection.SelectedValue is UnityEngine.Object unityObject)
                    Selection.activeObject = unityObject;
            }
        }

        private void OnSelectionConfirmed(OdinMenuTreeSelection selection)
        {
            Debug.Log("Selection confirmed" % Colorize.Red);
            if (selection.SelectedValue != null
            && selection.SelectedValue is UnityEngine.Object unityObject)
                Selection.activeObject = unityObject;
        }
    }

    private class PresetsExplorerDisplayParameters
    {
        public readonly AbilitySystemAnalyzer Analyzer;

        public PresetsExplorerDisplayParameters(AbilitySystemAnalyzer analyzer)
        {
            Analyzer = analyzer;
        }

        [TitleGroup("Presets Display Parameters")]
        [OnValueChanged("@" + nameof(OnDisplayFlatChanged) + "()")]
        [HideLabel, Title("Flatten Hierarchy", HorizontalLine = false)]
        [ShowInInspector, OdinSerialize]
        public bool FlattenHierarchy { get; set; }

        private void OnDisplayFlatChanged()
        {
            if (Analyzer == null)
                return;
            Analyzer.ForceMenuTreeRebuild();
        }
    }
    #endregion

    private const string PresetsPath = @"Assets/Battle/Presets";

    [OdinSerialize]
    private PresetsExplorerDisplayParameters ExplorerParameters;

    [MenuItem("Tools/Order Elimination/Ability System Analyzer")]
    public static void OpenWindow()
    {
        var window = GetWindow<AbilitySystemAnalyzer>();
        window.ExplorerParameters = new(window);
        window.Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var abilitiesPath = $"{PresetsPath}/Abilities";
        var effectsPath = $"{PresetsPath}/Effects";
        var charactersPath = $"{PresetsPath}/Characters";
        var structuresPath = $"{PresetsPath}/Structures";

        DrawMenuSearchBar = true;
        var isFlat = ExplorerParameters.FlattenHierarchy;
        if (MenuTree != null)
        {
            MenuTree.Selection.SelectionConfirmed -= OnSelectionConfirmed;
        }

        var tree = new OdinMenuTree(false);
        tree.Add("Analyzer", new AbilitySystemAnalyzerInfo(), EditorIcons.SettingsCog);
        tree.Add("Presets", ExplorerParameters, EditorIcons.FileCabinet);
        tree.Add("Presets/Active Abilities", null, EditorIcons.Crosshair);
        tree.Add("Presets/Passive Abilities", null, EditorIcons.Clouds);
        tree.Add("Presets/Effects", null, EditorIcons.StarPointer);
        tree.Add("Presets/Characters", null, EditorIcons.Male);
        tree.Add("Presets/Structures", null, EditorIcons.House);

        tree.AddAllAssetsAtPath("Presets/Active Abilities", abilitiesPath, typeof(ActiveAbilityBuilder), true, isFlat)
            .SortMenuItemsByName()
            .AddIcons<ActiveAbilityBuilder>(b => b.Icon);
        tree.AddAllAssetsAtPath("Presets/Passive Abilities", abilitiesPath, typeof(PassiveAbilityBuilder), true, isFlat)
            .SortMenuItemsByName()
            .AddIcons<PassiveAbilityBuilder>(b => b.Icon);
        tree.AddAllAssetsAtPath("Presets/Effects", effectsPath, typeof(EffectDataPreset), true, isFlat)
            .SortMenuItemsByName()
            .AddIcons<EffectDataPreset>(b => b.View.Icon);
        tree.AddAllAssetsAtPath("Presets/Characters", charactersPath, typeof(CharacterTemplate), true, isFlat)
            .SortMenuItemsByName()
            .AddIcons<CharacterTemplate>(b => b.BattleIcon);
        tree.AddAllAssetsAtPath("Presets/Structures", structuresPath, typeof(StructureTemplate), true, isFlat)
            .SortMenuItemsByName()
            .AddIcons<StructureTemplate>(b => b.BattleIcon);

        tree.Selection.SelectionConfirmed += OnSelectionConfirmed;

        return tree;
    }

    private void OnSelectionConfirmed(OdinMenuTreeSelection selection)
    {
        if (selection.SelectedValue != null
            && selection.SelectedValue is UnityEngine.Object unityObject)
            Selection.activeObject = unityObject;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDestroy()
    {
        if (MenuTree != null)
        {
            MenuTree.Selection.SelectionConfirmed -= OnSelectionConfirmed;
        }
        base.OnDestroy();
    }
}
