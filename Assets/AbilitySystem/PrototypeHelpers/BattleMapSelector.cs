using DefaultNamespace;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UIManagement;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using OrderElimination;
using UnityEditor;

public enum SelectorMode
{
    SelectingUnit,
    SelectingTargets
}

public class BattleMapSelector : MonoBehaviour
{
    [TitleGroup("Components")]
    [SerializeField]
    private AbilityPanel _abilityPanel;

    [TitleGroup("Components")]
    [SerializeField]
    private CharacterBattleStatsPanel _characterBattleStatsPanel;

    [TitleGroup("Components")]
    [SerializeField]
    private TargetingSystemDisplay _targetingSystemDisplay;

    [TitleGroup("Components")]
    [SerializeField]
    private AbilityPreviewDisplayer _abilityPreviewDisplayer;

    private TextEmitter _textEmitter;

    [TitleGroup("Cell Visuals")]
    [SerializeField]
    private Color _availableCellsTint;

    [TitleGroup("Cell Visuals")]
    [SerializeField]
    private Color _forbidenCellsTint;

    [TitleGroup("Entity Visuals")]
    [SerializeField]
    private Color _playerHighlightColor;

    [TitleGroup("Entity Visuals")]
    [SerializeField]
    private Color _alliesHighlightColor;

    [TitleGroup("Entity Visuals")]
    [SerializeField]
    private Color _enemiesHighlightColor;

    [TitleGroup("Entity Visuals")]
    [SerializeField]
    private Color _othersHighlightColor;

    [TitleGroup("Prototyping")]
    [SerializeField]
    private bool _allowOnlyCurrentSideEntities;

    [TitleGroup("Prototyping")]
    [SerializeField]
    private bool _allowOnlyPlayerEntities;

    [TitleGroup("Prototyping")]
    [SerializeField]
    private bool _lockCharacterWhileCasting;

    [TitleGroup("Prototyping")]
    [SerializeField]
    private bool _confirmTargetingBySecondClick;

    private BattleMapView _battleMapView;
    private IBattleMap _battleMap => _battleContext.BattleMap;
    private IBattleContext _battleContext;
    private SelectorMode _mode;

    private CellView _lastClickedCell;
    private int _currentLoopIndex;

    private HashSet<Vector2Int> _availableCellsForTargeting;
    private AbilitySystemActor _currentSelectedEntity;
    private ActiveAbilityRunner _selectedAbility;

    private bool _isEnabled = true;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _battleContext = objectResolver.Resolve<IBattleContext>();
        _battleMapView = objectResolver.Resolve<BattleMapView>();
        _textEmitter = objectResolver.Resolve<TextEmitter>();
        _battleMapView.CellClicked += OnCellClicked;
        _battleContext.NewTurnStarted -= OnNewTurnStarted;
        _battleContext.NewTurnStarted += OnNewTurnStarted;

        void OnNewTurnStarted(IBattleContext battleContext)
        {
            DeselectEntity();
        }
    }

    public void Enable()
    {
        _isEnabled = true;
    }

    public void Disable()
    {
        DeselectEntity();
        _isEnabled = false;
    }

    private void OnCellClicked(CellView cellView)
    {
        if (!_isEnabled) return;
        if (_lockCharacterWhileCasting
            && _currentSelectedEntity != null
            && _currentSelectedEntity.ActiveAbilities.Any(r => r.IsRunning))
            return;
        if (_lastClickedCell != cellView)
            _currentLoopIndex = 0;
        _lastClickedCell = cellView;
        if (_mode == SelectorMode.SelectingUnit)
        {
            //Cell Selection Priority (Loops through):
            //1. Select Player-controlled Characters    (Abilities (clickable) + LeftPanel)
            //2. Select ally Characters                 (Abilities (unclickable) + LeftPanel)
            //3. Select enemy+neutral Characters        (Right panel)
            //4. Select Objects                         (???)
            _abilityPanel.ResetAbilityButtons();
            _characterBattleStatsPanel.HideInfo();

            var clickedPosition = _battleContext.BattleMap.GetPosition(cellView.Model);
            var entities = _battleContext.GetVisibleEntitiesAt(clickedPosition, BattleSide.Player)
                .OrderBy(e => e.EntityType)
                .ThenBy(e => e.BattleSide)//ThenBy controlling Player
                .ToArray();
            if (_allowOnlyCurrentSideEntities)
                entities = entities.Where(e => e.BattleSide == _battleContext.ActiveSide).ToArray();
            if (_allowOnlyPlayerEntities)
                entities = entities.Where(e => e.BattleSide == BattleSide.Player).ToArray();
            DeselectEntity();
            if (entities.Length > 0)
            {
                _currentLoopIndex %= entities.Length;
                var selectedEntity = entities[_currentLoopIndex];
                SelectEntity(selectedEntity);
                _currentLoopIndex++;

                if (selectedEntity.EntityType == EntityType.Character)
                {
                    if (selectedEntity.BattleSide == BattleSide.Player)
                    {
                        //
                    }
                    else if (selectedEntity.BattleSide == BattleSide.Enemies)
                    {
                        //
                    }
                }
                if (selectedEntity.EntityType == EntityType.Structure)
                {
                    //
                }
            }
        }
        else if (_mode == SelectorMode.SelectingTargets)
        {
            var cellPosition = _battleMap.GetPosition(cellView.Model);
            if (_selectedAbility.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem manualTargetingSystem)
            {
                if (manualTargetingSystem.SelectedCells.Contains(cellPosition))
                {
                    //_targetingSystemDisplay.HideCrosshair(cellPosition);
                    //second click
                    if (_confirmTargetingBySecondClick)
                    {
                        CastCurrentAbility();
                        return;
                    }
                    manualTargetingSystem.Deselect(cellPosition);
                }
                else
                {
                    if (manualTargetingSystem.Select(cellPosition))
                    {
                        //_targetingSystemDisplay.ShowCrosshair(cellPosition);
                    }
                    else
                        Logging.Log($"Wrong target at {cellPosition}");
                }
            }
            else if (_selectedAbility.AbilityData.TargetingSystem is NoTargetTargetingSystem noTargetSystem)
            {
                if (_currentSelectedEntity.Position == cellPosition && _confirmTargetingBySecondClick)
                    CastCurrentAbility();
            }
            if (_selectedAbility != null)
            {
                _abilityPreviewDisplayer.DisplayPreview(
                _selectedAbility.AbilityData,
                _currentSelectedEntity,
                _selectedAbility.AbilityData.TargetingSystem.ExtractCastTargetGroups());
            }
        }
    }

    private void SelectEntity(AbilitySystemActor entity)
    {
        DeselectEntity();
        entity.DisposedFromBattle += OnSelectedEntityDisposed;
        entity.AbilitiesChanged += e => _abilityPanel.AssignAbilities(e, entity.ActiveAbilities.ToArray());
        _currentSelectedEntity = entity;
        var view = _battleContext.EntitiesBank.GetViewByEntity(entity);
        _abilityPanel.AssignAbilities(entity, entity.ActiveAbilities.ToArray());
        _abilityPanel.AbilitySelected += OnAbilitySelect;
        _abilityPanel.AbilityDeselected += OnAbilityDeselect;
        foreach (var ability in entity.ActiveAbilities)
        {
            ability.AbilityExecutionStarted -= OnAbilityUpdated;
            ability.AbilityExecutionStarted += OnAbilityUpdated;
            ability.AbilityExecutionCompleted -= OnAbilityUpdated;
            ability.AbilityExecutionCompleted += OnAbilityUpdated;
        }
        _characterBattleStatsPanel.UpdateEntityInfo(view);
        _characterBattleStatsPanel.ShowInfo();
        var highlightColor = entity.BattleSide switch
        {
            BattleSide.Player => _playerHighlightColor,
            BattleSide.Enemies => _enemiesHighlightColor,
            BattleSide.Allies => _alliesHighlightColor,
            BattleSide.Others => _enemiesHighlightColor,
            _ => _othersHighlightColor,
        };
        view.Highlight(highlightColor);
        Object so = entity.EntityType == EntityType.Character
            ? (Object)_battleContext.EntitiesBank.GetBasedCharacter(entity).CharacterData
            : (Object)_battleContext.EntitiesBank.GetBasedStructureTemplate(entity);
        EditorUtility.IsPersistent(so);
        Debug.Log(
            $"{entity.EntityType} {view.Name} selected." % Colorize.ByColor(new Color(1, 0.5f, 0.5f))
            + $"\nActionPoints: {string.Join(", ", entity.EnergyPoints.Select(e => $"[{e.Key}:{e.Value}]"))}"
            + $"\nHealth: {entity.BattleStats.Health}; MaxHealth: {entity.BattleStats.MaxHealth.ModifiedValue}" 
            + $"\nTotalArmor: {entity.BattleStats.TotalArmor}; MaxArmor: {entity.BattleStats.MaxArmor.ModifiedValue}" 
            + $"\nDamage: {entity.BattleStats[BattleStat.AttackDamage].ModifiedValue};" 
            + $"\nAccuracy: {entity.BattleStats[BattleStat.Accuracy].ModifiedValue};" 
            + $"\nEvasion: {entity.BattleStats[BattleStat.Evasion].ModifiedValue};" 
            + $"\n{entity.StatusHolder}"
            + $"\nEffects({entity.Effects.Count()}): {string.Join(", ", entity.Effects.Select(e => $"[{e.EffectData.View.Name}]"))}");
    }

    private void DeselectEntity()
    {
        _abilityPanel.ResetAbilityButtons();
        _abilityPanel.AbilitySelected -= OnAbilitySelect;
        _abilityPanel.AbilityDeselected -= OnAbilityDeselect; 
        if (_currentSelectedEntity != null)
        {
            _currentSelectedEntity.DisposedFromBattle -= OnSelectedEntityDisposed;
            foreach (var ability in _currentSelectedEntity.ActiveAbilities)
            {
                ability.AbilityExecutionStarted -= OnAbilityUpdated;
                ability.AbilityExecutionCompleted -= OnAbilityUpdated;
            }
        }
        _characterBattleStatsPanel.HideInfo();
        _currentSelectedEntity = null;
    }

    private void OnAbilitySelect(ActiveAbilityRunner abilityRunner)
    {
        if (_selectedAbility != null)
            return;//throw new System.InvalidOperationException("Only one ability can be selected at once. Deselect first.");
        if (_currentSelectedEntity != null
            && !abilityRunner.IsCastAvailable(_battleContext, _currentSelectedEntity))
            return;
        var targetingSystem = abilityRunner.AbilityData.TargetingSystem;
        var targetRequiringSystem = targetingSystem as IRequireSelectionTargetingSystem;
        if (targetRequiringSystem != null)
        {
            targetRequiringSystem.SelectionUpdated -= OnSelectionUpdated;
            targetRequiringSystem.SelectionUpdated += OnSelectionUpdated;
            targetRequiringSystem.ConfirmationUnlocked -= OnConfirmationUnlocked;
            targetRequiringSystem.ConfirmationUnlocked += OnConfirmationUnlocked;
            targetRequiringSystem.ConfirmationLocked -= OnConfirmationLocked;
            targetRequiringSystem.ConfirmationLocked += OnConfirmationLocked;
        }
        abilityRunner.InitiateCast(_battleContext, _currentSelectedEntity);
        if (targetRequiringSystem != null)
        {
            _availableCellsForTargeting = targetRequiringSystem.CurrentAvailableCells.ToHashSet();
        }
        _mode = SelectorMode.SelectingTargets;
        _selectedAbility = abilityRunner;
        HighlightCells();
        _abilityPreviewDisplayer.DisplayPreview(
                _selectedAbility.AbilityData,
                _currentSelectedEntity,
                _selectedAbility.AbilityData.TargetingSystem.ExtractCastTargetGroups());
    }

    private void OnAbilityDeselect(ActiveAbilityRunner abilityRunner)
    {
        if (_selectedAbility == null)
            return;//throw new System.InvalidOperationException("There is no ability selected.");
        _targetingSystemDisplay.HideAllVisuals();
        _abilityPreviewDisplayer.HidePreview();
        if (abilityRunner.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem targetingSystem)
        {
            targetingSystem.SelectionUpdated -= OnSelectionUpdated;
            targetingSystem.ConfirmationUnlocked -= OnConfirmationUnlocked;
            targetingSystem.ConfirmationLocked -= OnConfirmationLocked;
            _availableCellsForTargeting = null;
        }
        _battleMapView.DelightCells();
        _mode = SelectorMode.SelectingUnit;
        _selectedAbility.AbilityData.TargetingSystem.CancelTargeting();
        _selectedAbility = null;
    }

    private void OnAbilityUpdated(ActiveAbilityRunner abilityRunner)
    {
        _abilityPanel.UpdateAbilityButtonsAvailability();
    }

    private void OnSelectionUpdated(IRequireSelectionTargetingSystem targetingSystem)
    {
        HighlightCells();
        //Applies a slight tint on cells which player pressed while selecting targets
        //Unnesessary since can be done through cell group colors
        var selectedCells = targetingSystem.SelectedCells.ToArray();
        if (_selectedAbility.AbilityData.View.ShowCrosshairWhenTargeting)
            _targetingSystemDisplay.ShowCrosshairs(selectedCells);
        if (_selectedAbility.AbilityData.View.ShowTrajectoryWhenTargeting)
            _targetingSystemDisplay.ShowTrajectories(
                selectedCells
                .Select(p => new Vector2IntSegment(_currentSelectedEntity.Position, p))
                .ToArray());
        foreach (var pos in selectedCells)
        {
            var cellView = _battleMapView.GetCell(pos.x, pos.y);
            _battleMapView.HighlightCell(pos.x, pos.y, cellView.CurrentColor * 0.8f);
        }
    }

    private void OnSelectedEntityDisposed(IBattleDisposable entity)
    {
        if (entity != _currentSelectedEntity)
            throw new System.Exception("Should be unsubscribed from not selected entities.");
        DeselectEntity();
    }

    private void HighlightCells()
    {
        var targetedCells = _selectedAbility.AbilityData.TargetingSystem.ExtractCastTargetGroups();
        _battleMapView.DelightCells();
        if (_selectedAbility.AbilityData.TargetingSystem is IRequireSelectionTargetingSystem)
        {
            foreach (var pos in _battleMap.CellRangeBorders.EnumerateCellPositions())
            {
                _battleMapView.HighlightCell(pos.x, pos.y, _forbidenCellsTint);
            }
            foreach (var pos in _availableCellsForTargeting)
            {
                _battleMapView.HighlightCell(pos.x, pos.y, _availableCellsTint);
            }
        }
        var colors = _selectedAbility.AbilityData.View.TargetGroupsHighlightColors;
        foreach (var group in targetedCells.ContainedCellGroups)
        {
            foreach (var pos in targetedCells.GetGroup(group))
            {
                var currentColor = _battleMapView.GetCell(pos.x, pos.y).CurrentColor;
                var newColor = Color.Lerp(currentColor, colors[group], colors[group].a);
                _battleMapView.HighlightCell(pos.x, pos.y, newColor);
            }
        }
    }

    private void OnConfirmationUnlocked(IRequireSelectionTargetingSystem multiTargetSystem)
    {
        Debug.Log("Ability use ready.");
    }
    private void OnConfirmationLocked(IRequireSelectionTargetingSystem multiTargetSystem)
        => Debug.Log("Ability use locked.");

    private void CastCurrentAbility()
    {
        if (_selectedAbility != null && _selectedAbility.AbilityData.TargetingSystem.IsConfirmAvailable)
        {
            var abilityName = _selectedAbility.AbilityData.View.Name;
            //Debug.Log($"Ability �{abilityName}� has been used." % Colorize.Cyan);
            _selectedAbility.AbilityData.TargetingSystem.ConfirmTargeting();
        }
    }
    #region ToRemove
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 1.0f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 0.25f;
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            Time.timeScale *= 2;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            Time.timeScale /= 2;
        }
        #if !UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("We're cool" % Colorize.Blue);
            Application.Quit();
        }
        #endif
    }
    #endregion
}
