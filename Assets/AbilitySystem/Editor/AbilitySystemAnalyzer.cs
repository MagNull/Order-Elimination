using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Editor;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using static OrderElimination.Infrastructure.ReflectionExtensions;

public class AbilitySystemAnalyzer : OdinMenuEditorWindow
{
    #region SupportTypes
    private class AbilitySystemAnalyzerInfo
    {
        private enum SearchForOption
        {
            TypeReference,
            InstanceReference
        }

        private bool IsTypeSearch => SearchFor == SearchForOption.TypeReference;
        private bool IsInstanceSearch => SearchFor == SearchForOption.InstanceReference;
        private bool InstanceHandlerIsValid
        {
            get
            {
                if (InstanceHandler == null
                    || IsTypeSearch && SeekingType == null
                    || IsInstanceSearch && SeekingInstance == null)
                    return false;
                var instanceType = SearchFor switch
                {
                    SearchForOption.TypeReference => SeekingType,
                    SearchForOption.InstanceReference => SeekingInstance.GetType(),
                    _ => throw new NotImplementedException(),
                };
                return true;
                //var changerInputType = InstanceHandler.GetType().GetGenericArguments()[0];
                //return changerInputType.IsAssignableFrom(instanceType);
            }
        }
        private bool HasResults
            => SearchResult != null && SearchResult.MenuItems.Count > 0;

        private ActiveAbilityBuilder[] _projectActiveAbilities;
        private PassiveAbilityBuilder[] _projectPassiveAbilities;
        private EffectDataPreset[] _projectEffects;
        private CharacterTemplate[] _projectCharacters;
        private StructureTemplate[] _projectStructures;
        private AnimationPreset[] _projectAnimationPresets;

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

        [TitleGroup("Statistics")]
        [ShowInInspector]
        public int TotalAnimationPresets => _projectAnimationPresets.Length;

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ValidateInput("@false", "Only searches for assigned serialized values")]
        [EnumToggleButtons]
        [ShowInInspector]
        private SearchForOption SearchFor { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [EnableIf("@SearchFor == SearchForOption.TypeReference")]
        [ShowInInspector]
        public Type SeekingType { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [EnableIf("@" + nameof(IsInstanceSearch))]
        [ShowInInspector]
        public object SeekingInstance { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowInInspector]
        public bool DirectReferenceOnly { get; set; } = true;

        //public bool IncludeNullEntries = false

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowInInspector]
        public bool SearchInAnimations { get; set; } = true;

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [ShowInInspector]
        public IValueEntryFilter EntriesFilter { get; set; }

        [TitleGroup("Dependencies Search")]
        [VerticalGroup("Dependencies Search/Parameters")]
        [GUIColor(1, 0.5f, 0.5f)]
        [ShowInInspector]
        public bool SuppressErrors { get; set; } = false;

        [VerticalGroup("Dependencies Search/Results")]
        [ShowIf("@" + nameof(HasResults))]
        //[HideLabel, Title("Auto Select Asset")]
        [ShowInInspector]
        public bool AutoSelectAsset { get; set; } = false;

        [VerticalGroup("Dependencies Search/Results")]
        [ShowIf("@" + nameof(HasResults))]
        [HideLabel, OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector]
        private OdinMenuTree SearchResult { get; set; }

        private List<ScriptableObject> _foundAssets = new();
        private Dictionary<ScriptableObject, SerializedMember[]>  _foundTypeEntries = new();
        private Dictionary<ScriptableObject, Exception> _assetsWithExceptions = new();

        [ValidateInput("@" + nameof(InstanceHandlerIsValid), "Invalid " + nameof(InstanceHandler))]
        [VerticalGroup("Dependencies Search/Replace")]
        [ShowInInspector]
        private IValueEntryHandler InstanceHandler { get; set; }

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
            _projectAnimationPresets = AssetDatabase
                .FindAssets($"t: {nameof(AnimationPreset)}")
                .Select(id => AssetDatabase.GUIDToAssetPath(id))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(AnimationPreset)) as AnimationPreset)
                .Where(e => e != null)
                .ToArray();
        }

        [VerticalGroup("Dependencies Search/Parameters")]
        [PropertyTooltip(
            "Cascaded search for serialized value entries of seeking type.")]
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
                .Concat(_projectAnimationPresets)
                .ToArray();
            var foundAssets = new List<ScriptableObject>();
            var foundTypeEntries = new Dictionary<ScriptableObject, SerializedMember[]>();
            var assetsWithExceptions = new Dictionary<ScriptableObject, Exception>();
            //var ignoredExceptionLimit = 10;
            var seekingType = SearchFor switch
            {
                SearchForOption.TypeReference => SeekingType,
                SearchForOption.InstanceReference => SeekingInstance.GetType(),
                _ => throw new NotImplementedException(),
            };

            if (SuppressErrors)
            {
                foreach (var asset in assets)
                {
                    try
                    {
                        var members = GetAssignedMembersOfType(asset, seekingType).ToArray();
                        if (members.Length > 0)
                        {
                            foundAssets.Add(asset);
                            foundTypeEntries.Add(asset, members);
                        }
                    }
                    catch (Exception e)
                    {
                        Logging.LogError($"Exception on {asset.name}");
                        assetsWithExceptions.Add(asset, e);
                        break;
                        throw e;
                    }
                }
            }
            else
            {
                foreach (var asset in assets)
                {
                    var members = GetAssignedMembersOfType(asset, seekingType).ToArray();
                    if (members.Length > 0)
                    {
                        foundAssets.Add(asset);
                        foundTypeEntries.Add(asset, members);
                    }
                }
            }

            _assetsWithExceptions = assetsWithExceptions;

            if (SearchFor == SearchForOption.InstanceReference)
            {
                FilterAssetsByFoundMembers(m => m.MemberValue == SeekingInstance);
            }

            if (EntriesFilter != null)
            {
                FilterAssetsByFoundMembers(m => EntriesFilter.IsAllowed(m));
            }

            _foundAssets = foundAssets;
            _foundTypeEntries = foundTypeEntries;

            DisplaySearchResults();

            void FilterAssetsByFoundMembers(Func<SerializedMember, bool> selector)
            {
                var filteredAssets = new List<ScriptableObject>();
                var filteredTypeEntries = new Dictionary<ScriptableObject, SerializedMember[]>();
                foreach (var asset in foundAssets)
                {
                    var members = foundTypeEntries[asset].Where(selector).ToArray();
                    if (members.Length > 0)
                    {
                        filteredAssets.Add(asset);
                        filteredTypeEntries.Add(asset, members);
                    }
                }
                foundAssets = filteredAssets;
                foundTypeEntries = filteredTypeEntries;
            }
        }

        private void DisplaySearchResults()
        {
            if (SearchResult != null)
            {
                SearchResult.Selection.SelectionConfirmed -= OnSelectionConfirmed;
                SearchResult.Selection.SelectionChanged -= OnSelectionChanged;
            }
            var searchTree = new OdinMenuTree(false);
            searchTree.Selection.SelectionConfirmed += OnSelectionConfirmed;
            searchTree.Selection.SelectionChanged += OnSelectionChanged;
            SearchResult = searchTree;
            AddToSearchResultDisplay(
                $"Found Assets ({_foundAssets.Count}) with {_foundTypeEntries.Sum(e => e.Value.Length)} entries", _foundAssets, _foundTypeEntries);
            AddToSearchResultDisplay(
                $"Assets with Exceptions ({_assetsWithExceptions.Count})", _assetsWithExceptions.Keys, _foundTypeEntries);
            if (_assetsWithExceptions.Count > 0)
            {
                Logging.LogError(
                    $"Assets with exceptions ({_assetsWithExceptions.Count}):\n\t" % Colorize.Red
                    + string.Join("\n\t", _assetsWithExceptions.Select(a => $"Asset with exception: \"{a.Key.name}\": {a.Value.GetExceptionName()}"))
                    + "\n");
            }
        }

        [VerticalGroup("Dependencies Search/Replace")]
        [Button, GUIColor("@Color.red")]
        public void ChangeValueEntries()
        {
            if (InstanceHandler == null)
                throw new ArgumentNullException($"{nameof(InstanceHandler)} is null");
            var seekingType = SearchFor switch
            {
                SearchForOption.TypeReference => SeekingType,
                SearchForOption.InstanceReference => SeekingInstance?.GetType(),
                _ => throw new NotImplementedException(),
            };
            if (seekingType == null)
                throw new ArgumentNullException($"Type is null");
            if (seekingType.IsValueType)
                throw new NotSupportedException("Value types not supported.");
            if (!InstanceHandlerIsValid)
                throw new InvalidOperationException($"Invalid {InstanceHandler}");
            FindAssignedDependencies();
            Undo.RecordObjects(_foundAssets.ToArray(), "Value entries modified");
            foreach (var asset in _foundAssets)
            {
                var isDirty = EditorUtility.IsDirty(asset);
                EditorUtility.SetDirty(asset);
                foreach (var entry in _foundTypeEntries[asset])
                    InstanceHandler.HandleValueEntry(entry);
                PrefabUtility.RecordPrefabInstancePropertyModifications(asset);
                if (!isDirty)
                    EditorUtility.ClearDirty(asset);
            }
            Undo.RecordObjects(_foundAssets.ToArray(), "Value entries modified");
        }

        private IEnumerable<SerializedMember> GetAssignedMembersOfType(object where, Type desiredMemberType)
        {
            //var rootName = where is ScriptableObject whereSO ? whereSO.name : "<asset>";
            var rootMember = new SerializedMember("<asset>", where, null);
            var openedAssets = new HashSet<ScriptableObject>();
            return ReflectionExtensions.GetAllSerializedMembersOfType(rootMember, desiredMemberType, StopAt);

            bool StopAt(object e)
            {
                if (e.IsAbilitySystemAsset() || e is AnimationPreset)
                {
                    if (DirectReferenceOnly)
                        return true;
                    if (!openedAssets.Contains(e))
                    {
                        openedAssets.Add((ScriptableObject)e);
                        return false;
                    }
                    else return true;
                }
                if (e is IAbilityAnimation)
                {
                    return !SearchInAnimations;
                }
                return false;
            }
        }

        private void AddToSearchResultDisplay(
            string categoryName, 
            IEnumerable<ScriptableObject> assets, 
            IDictionary<ScriptableObject, SerializedMember[]> entries)
        {
            foreach (var foundAsset in assets)
            {
                var entriesCount = entries[foundAsset].Length > 1
                    ? $"{entries[foundAsset].Length} entries"
                    : "1 entry";
                var assetName = $"{foundAsset.name} ({entriesCount})";
                if (foundAsset is ActiveAbilityBuilder activeAbility)
                {
                    SearchResult.Add($"{categoryName}/Active Abilities/{assetName}", foundAsset, activeAbility.Icon);
                }
                else if (foundAsset is PassiveAbilityBuilder passiveAbility)
                {
                    SearchResult.Add($"{categoryName}/Passive Abilities/{assetName}", foundAsset, passiveAbility.Icon);
                }
                else if (foundAsset is EffectDataPreset effect)
                {
                    SearchResult.Add($"{categoryName}/Effects/{assetName}", foundAsset, effect.View.Icon);
                }
                else if (foundAsset is CharacterTemplate character)
                {
                    SearchResult.Add($"{categoryName}/Characters/{foundAsset.name}", foundAsset, character.BattleIcon);
                }
                else if (foundAsset is StructureTemplate structure)
                {
                    SearchResult.Add($"{categoryName}/Structures/{assetName}", foundAsset, structure.BattleIcon);
                }
                else if (foundAsset is AnimationPreset animationPreset)
                {
                    SearchResult.Add($"{categoryName}/Animations/{assetName}", foundAsset);
                }
                else
                {
                    SearchResult.Add($"{categoryName}/Unknown/{assetName}", foundAsset);
                }
            }
            var menuItem = SearchResult.GetMenuItem(categoryName);
            menuItem?.MenuTree.SortMenuItemsByName();
        }

        private void OnSelectionChanged(SelectionChangedType changeType)
        {
            if (changeType == SelectionChangedType.ItemAdded)
            {
                if (SearchResult.Selection.SelectedValue != null
                    && SearchResult.Selection.SelectedValue is UnityEngine.Object unityObject)
                {
                    if (AutoSelectAsset)
                        Selection.activeObject = unityObject;
                    if (unityObject.IsAbilitySystemAsset())
                    {
                        var asset = (ScriptableObject)unityObject;

                        Debug.Log($"{asset.GetType().Name} «{asset.name}» with {_foundTypeEntries[asset].Length} entries:\n - "
                            + string.Join($"\n - ", _foundTypeEntries[asset].Select(m => m.GetFullName()))
                            + "\n");
                    }
                }
            }
        }

        private void OnSelectionConfirmed(OdinMenuTreeSelection selection)
        {
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

    #region Assets Inspector
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
    #endregion
}
