using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Battle;
using OrderElimination.Editor;
using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        private bool HasResults => SearchResult?.MenuItems.Count > 0;

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
        [InfoBox("Only searches for assigned serialized values")]
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
            _projectActiveAbilities = AssetsUtility.GetAllAssetsOfType<ActiveAbilityBuilder>();
            _projectPassiveAbilities = AssetsUtility.GetAllAssetsOfType<PassiveAbilityBuilder>();
            _projectEffects = AssetsUtility.GetAllAssetsOfType<EffectDataPreset>();
            _projectCharacters = AssetsUtility.GetAllAssetsOfType<CharacterTemplate>();
            _projectStructures = AssetsUtility.GetAllAssetsOfType<StructureTemplate>();
            _projectAnimationPresets = AssetsUtility.GetAllAssetsOfType<AnimationPreset>();
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
            return GetAllSerializedMembersOfType(rootMember, desiredMemberType, StopAt);

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
            var assetsArray = assets.ToArray();
            //var assetsCount = assetsArray.GroupBy(a => a.GetType()).ToDictionary(g => g.Key, g => g.Count());
            foreach (var foundAsset in assetsArray)
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

    [Serializable]
    private class TemplatesExplorer
    {
        private bool _flattenHierarchy = true;
        private bool _orderByName = true;
        private string _gameContentPath = DefaultGameContentPath;

        [Title("Presets Display Parameters")]
        [PropertyOrder(-1)]
        [ShowInInspector]
        public bool UseFolderHierarchy
        {
            get => _flattenHierarchy;
            set
            {
                if (_flattenHierarchy == value) return;
                _flattenHierarchy = value;
                HierarchyDisplayParametersChanged?.Invoke(this);
            }
        }

        [PropertyOrder(-1)]
        [ShowInInspector]
        public bool OrderByName
        {
            get => _orderByName;
            set
            {
                if (_orderByName == value) return;
                _orderByName = value;
                HierarchyDisplayParametersChanged?.Invoke(this);
            }
        }

        [VerticalGroup("ContentPath")]
        [ShowInInspector]
        public const string DefaultGameContentPath = @"Assets/Resources/Battle/Presets";

        [VerticalGroup("ContentPath")]
        [DelayedProperty]
        [ShowInInspector]
        public string GameContentPath
        {
            get => _gameContentPath;
            set
            {
                var exists = UnityEngine.Windows.Directory.Exists(value);
                if (!exists)
                    return;
                _gameContentPath = value;
                HierarchyDisplayParametersChanged?.Invoke(this);
            }
        }

        [HorizontalGroup("ContentPath/Buttons")]
        [Button("Show in inspector")]
        public void ShowFolderInInspector()
        {
            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(GameContentPath);
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
        }

        [HorizontalGroup("ContentPath/Buttons")]
        [Button("Set to default")]
        public void SetPathToDefault()
        {
            GameContentPath = DefaultGameContentPath;
        }

        public event Action<TemplatesExplorer> HierarchyDisplayParametersChanged;

        public void DisplayGameAssets(OdinMenuTree menuTree)
        {
            var path = GameContentPath;
            var abilitiesPath = $"{path}/Abilities";
            var effectsPath = $"{path}/Effects";
            var charactersPath = $"{path}/Characters";
            var structuresPath = $"{path}/Structures";
            var mapsPath = $"{path}/Maps";

            DisplayAssets<ActiveAbilityBuilder>(
            menuTree, path, "Presets/Active Abilities", EditorIcons.Crosshair, a => a.Icon);
            DisplayAssets<PassiveAbilityBuilder>(
                menuTree, path, "Presets/Passive Abilities", EditorIcons.Clouds, a => a.Icon);
            DisplayAssets<EffectDataPreset>(
                menuTree, path, "Presets/Effects", EditorIcons.StarPointer, a => a.View.Icon);
            DisplayAssets<CharacterTemplate>(
                menuTree, path, "Presets/Characters", EditorIcons.Male, a => a.BattleIcon);
            DisplayAssets<StructureTemplate>(
                menuTree, path, "Presets/Structures", EditorIcons.House, a => a.BattleIcon);
            menuTree.Add("Presets/Maps", null, EditorIcons.Globe);
            DisplayAssets<BattleScenario>(
                menuTree, path, "Presets/Maps/BattleScenario", EditorIcons.GridBlocks, a => null);
            DisplayAssets<BattleFieldLayeredLayout>(
                menuTree, path, "Presets/Maps/Layered Layout", EditorIcons.GridLayout, a => null);
        }

        //TODO: remove iconGetter, use "where TAsset : ITemplateAsset"
        private IEnumerable<OdinMenuItem> DisplayAssets<TAsset>(
            OdinMenuTree menuTree,
            string assetsPath, string displayPath,
            EditorIcon rootIcon, Func<TAsset, Sprite> iconGetter)
            where TAsset : class
        {
            //var subElements = new List<TAsset>();
            menuTree.Add(displayPath, null, rootIcon);
            var createdMenuItems =
                menuTree.AddAllAssetsAtPath(displayPath, assetsPath, typeof(TAsset), true, !UseFolderHierarchy)
                .AddIcons<TAsset>(b => iconGetter(b));
            if (OrderByName)
                createdMenuItems.SortMenuItemsByName();
            //foreach (var e in createdMenuItems.Select(i => i.Value as TAsset).Where(a => a != null))
            //{
            //    subElements.Add(e);
            //}
            return createdMenuItems;
        }
    }

    private class GuidExplorer
    {
        [TitleGroup("Guid Management")]
        [ShowInInspector]
        private bool _safeMode = true;

        [TitleGroup("Guid Management")]
        [GUIColor("@Color.yellow")]
        [Button]
        private void ReplaceEmptyGuidsWithRandom()
        {
            var searchResult = FindGuidAssets(a => a.AssetId == Guid.Empty);
            AssignRandomGuids(searchResult.Values.SelectMany(a => a));
            //Update search result
            DisplaySearchResult(FindGuidAssets(a => true));
        }

        [TitleGroup("Guid Management")]
        [GUIColor("@Color.red")]
        [DisableIf("@" + nameof(_safeMode))]
        [Button]
        private void AssignAllAssetsRandomGuid()
        {
            var searchResult = FindGuidAssets(a => true);
            AssignRandomGuids(searchResult.Values.SelectMany(a => a));
            //Update search result
            DisplaySearchResult(FindGuidAssets(a => true));
        }

        [TitleGroup("Guid Search")]
        [Button]
        private void FindAllGuidAssets()
        {
            DisplaySearchResult(FindGuidAssets(a => true));
        }

        [TitleGroup("Guid Search")]
        [Button]
        private void FindEmptyGuidAssets()
        {
            var emptyGuid = Guid.Empty;
            DisplaySearchResult(FindGuidAssets(a => a.AssetId == emptyGuid));
        }

        [TitleGroup("Guid Search")]
        [InfoBox("Searches for " + nameof(IGuidAsset) + " assets")]
        [PropertyOrder(1)]
        [ShowInInspector, OdinSerialize]
        private string _guidToSearch = string.Empty;

        [TitleGroup("Guid Search")]
        [PropertyOrder(2)]
        [Button]
        private void FindAssetsByGuid()
        {
            if (!Guid.TryParse(_guidToSearch, out var guid))
            {
                Logging.LogError("Wrong GUID format");
                return;
            }
            DisplaySearchResult(FindAssetsByGuid(guid));
        }

        [TitleGroup("Guid Search")]
        [ShowIf("@" + nameof(HasResults))]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [PropertyOrder(3)]
        [ShowInInspector]
        private OdinMenuTree _searchResult;

        private bool HasResults => _searchResult?.MenuItems.Count > 0;

        private void AssignRandomGuids(IEnumerable<IGuidAsset> assets)
        {
            var unityAssets = assets
                .Select(a => a as UnityEngine.Object)
                .Where(a => a != null)
                .ToArray();
            Undo.RecordObjects(unityAssets, "Random GUIDs assigned");
            foreach (var asset in unityAssets)
            {
                var isDirty = EditorUtility.IsDirty(asset);
                EditorUtility.SetDirty(asset);
                ((IGuidAsset)asset).UpdateId(Guid.NewGuid());
                PrefabUtility.RecordPrefabInstancePropertyModifications(asset);
                if (!isDirty)
                    EditorUtility.ClearDirty(asset);
            }
        }

        private void DisplaySearchResult(IReadOnlyDictionary<Type, IGuidAsset[]> searchResult)
        {
            if (_searchResult != null)
            {
                _searchResult.Selection.SelectionChanged -= OnSelectionChanged;
                _searchResult.Selection.SelectionConfirmed -= OnSelectionConfirmed;
            }
            _searchResult = new();
            _searchResult.Selection.SelectionChanged += OnSelectionChanged;
            _searchResult.Selection.SelectionConfirmed += OnSelectionConfirmed;
            foreach (var item in searchResult)
            {
                var type = item.Key;
                var assets = item.Value;
                foreach (var asset in assets)
                {
                    var assetName = asset is UnityEngine.Object unityAsset
                        ? unityAsset.name
                        : "???";
                    _searchResult.Add($"{type.Name}/{assetName} ({asset.AssetId})", asset);
                }
            }
            foreach (var upperSection in _searchResult.MenuItems)
            {
                upperSection.Name = $"{upperSection.Name} ({upperSection.ChildMenuItems.Count})";
            }
        }

        private IReadOnlyDictionary<Type, IGuidAsset[]> FindAssetsByGuid(Guid guid)
        {
            return FindGuidAssets(a => a.AssetId == guid);
        }

        private IReadOnlyDictionary<Type, IGuidAsset[]> FindGuidAssets(Predicate<IGuidAsset> filter)
        {
            var guidTypes = GetAllInterfaceImplementationTypes<IGuidAsset>();
            var resultsByType = new Dictionary<Type, IGuidAsset[]>();
            foreach (var t in guidTypes)
            {
                var assetsOfType = AssetsUtility.GetAllAssetsByType(t)
                    .Cast<IGuidAsset>()
                    .Where(a => filter(a))
                    .ToArray();
                resultsByType.Add(t, assetsOfType);
            }
            return resultsByType;
        }

        private void OnSelectionChanged(SelectionChangedType changeType)
        {
            if (changeType == SelectionChangedType.ItemAdded)
            {
                if (_searchResult.Selection.SelectedValue != null
                    && _searchResult.Selection.SelectedValue is UnityEngine.Object unityObject)
                {
                    Selection.activeObject = unityObject;
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
    #endregion

    #region Assets Inspector
    [OdinSerialize]
    private TemplatesExplorer Explorer;

    [MenuItem("Tools/Order Elimination/Ability System Analyzer")]
    public static void OpenWindow()
    {
        var window = GetWindow<AbilitySystemAnalyzer>();
        if (window.Explorer == null)
        {
            window.Explorer = new();
            window.Explorer.HierarchyDisplayParametersChanged
                += explorer => window.ForceMenuTreeRebuild();
        }
        window.Show();
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        DrawMenuSearchBar = true;
        if (MenuTree != null)
        {
            MenuTree.Selection.SelectionConfirmed -= OnSelectionConfirmed;
        }

        var tree = new OdinMenuTree(false);
        tree.Add("Analyzer", new AbilitySystemAnalyzerInfo(), EditorIcons.SettingsCog);
        tree.Add("GUID Explorer", new GuidExplorer(), EditorIcons.Ruler);
        tree.Add("Presets", Explorer, EditorIcons.FileCabinet);
        Explorer.DisplayGameAssets(tree);

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
