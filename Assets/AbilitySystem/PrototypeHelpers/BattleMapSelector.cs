using Assets.AbilitySystem.PrototypeHelpers;
using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using TMPro.EditorUtilities;
using UIManagement;
using UIManagement.Elements;
using UnityEngine;
using VContainer;

public enum SelectorMode
{
    SelectingUnit,
    SelectingTargets
}

public class BattleMapSelector : MonoBehaviour
{
    [SerializeField]
    private AbilityPanel _abilityPanel;
    [SerializeField]
    private CharacterBattleStatsPanel _characterBattleStatsPanel;
    [SerializeField]
    private Color _availableCellsTint;
    [SerializeField]
    private Color _forbidenCellsTint;

    private BattleMapView _battleMapView;
    private BattleEntitiesBank _entitiesBank;
    private IBattleMap _battleMap => _battleContext.BattleMap;
    private IBattleContext _battleContext;
    private SelectorMode _mode;
    private HashSet<Vector2Int> _availableCellsForTargeting;

    private CellView _lastClickedCell;
    private IAbilitySystemActor _currentSelectedEntity;
    private int _currentLoopIndex;
    private AbilityRunner _selectedAbility;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _entitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
        _battleContext = objectResolver.Resolve<IBattleContext>();
        _battleMapView = objectResolver.Resolve<BattleMapView>();
        _battleMapView.CellClicked += OnCellClicked;
        _battleContext.NewRoundStarted -= OnNewRoundStarted;
        _battleContext.NewRoundStarted += OnNewRoundStarted;
        void OnNewRoundStarted(IBattleContext battleContext)
        {
            DeselectEntity();
        }
    }

    private void OnCellClicked(CellView cellView)
    {
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

            var entities = cellView.Model
                .GetContainingEntities()
                .OrderBy(e => e.EntityType)
                .ThenBy(e => e.BattleSide)//ThenBy controlling Player
                .ToArray();
            _currentSelectedEntity = null;
            if (entities.Length == 0)
                return;
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
            if (selectedEntity.EntityType == EntityType.MapObject)
            {
                //
            }
        }
        else if (_mode == SelectorMode.SelectingTargets)
        {
            var cellPosition = _battleMap.GetPosition(cellView.Model);
            if (_selectedAbility.AbilityData.TargetingSystem is MultiTargetTargetingSystem multiTargetSystem)
            {
                if (multiTargetSystem.RemoveFromSelection(cellPosition)) { }
                else if (multiTargetSystem.AddToSelection(cellPosition)) { }
                else
                    print($"Wrong target at {cellPosition}");
            }
            else if (_selectedAbility.AbilityData.TargetingSystem is SingleTargetTargetingSystem singleTargetSystem)
            {
                if (singleTargetSystem.Select(cellPosition)) { }
                else if (singleTargetSystem.Deselect(cellPosition)) { }
                else
                    print($"Wrong target at {cellPosition}");
            }
        }
    }

    private void SelectEntity(IAbilitySystemActor entity)
    {
        DeselectEntity();
        _currentSelectedEntity = entity;
        var view = _entitiesBank.GetViewByEntity(entity);
        _abilityPanel.AssignAbilities(entity, entity.ActiveAbilities.ToArray(), new AbilityRunner[0]);
        _abilityPanel.AbilitySelected += OnAbilitySelect;
        _abilityPanel.AbilityDeselected += OnAbilityDeselect;
        _characterBattleStatsPanel.UpdateEntityInfo(view);
        _characterBattleStatsPanel.ShowInfo();

        Debug.Log($"{entity.EntityType} {view.Name} selected. ActionPoints: {string.Join(", ", entity.ActionPoints.Select(e => $"[{e.Key}:{e.Value}]"))}" % Colorize.Red);
    }

    private void DeselectEntity()
    {
        _abilityPanel.ResetAbilityButtons();
        _abilityPanel.AbilitySelected -= OnAbilitySelect;
        _abilityPanel.AbilityDeselected -= OnAbilityDeselect;
        _characterBattleStatsPanel.HideInfo();
    }

    private void OnAbilitySelect(AbilityRunner abilityRunner)
    {
        if (_selectedAbility != null)
            return;//throw new System.InvalidOperationException("Only one ability can be selected at once. Deselect first.");
        if (_currentSelectedEntity != null
            && !abilityRunner.IsCastAvailable(_battleContext, _currentSelectedEntity))
            return;
        var targetingSystem = abilityRunner.AbilityData.TargetingSystem;
        var targetRequiringSystem = targetingSystem as IRequireTargetsTargetingSystem;
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
            _availableCellsForTargeting = targetRequiringSystem.AvailableCells.ToHashSet();
        }
        _mode = SelectorMode.SelectingTargets;
        _selectedAbility = abilityRunner;
        HighlightCells();
    }

    private void OnAbilityDeselect(AbilityRunner abilityRunner)
    {
        if (_selectedAbility == null)
            return;//throw new System.InvalidOperationException("There is no ability selected.");
        if (abilityRunner.AbilityData.TargetingSystem is IRequireTargetsTargetingSystem targetingSystem)
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

    private void OnSelectionUpdated(IRequireTargetsTargetingSystem targetingSystem)
    {
        HighlightCells();
        var selectedCells = new List<Vector2Int>();
        if (targetingSystem is MultiTargetTargetingSystem multiTargetSys)
            selectedCells = multiTargetSys.SelectedCells.ToList();
        else if (targetingSystem is SingleTargetTargetingSystem singleTargetSys)
            selectedCells.Add(singleTargetSys.SelectedCell.Value);
        foreach (var pos in selectedCells)
        {
            var cellView = _battleMapView.GetCell(pos.x, pos.y);
            _battleMapView.HighlightCell(pos.x, pos.y, cellView.CurrentTint * 0.8f);
        }
    }

    private void HighlightCells()
    {
        var targetedCells = _selectedAbility.AbilityData.TargetingSystem.ExtractCastTargetGroups();
        _battleMapView.DelightCells();
        if (_selectedAbility.AbilityData.TargetingSystem is IRequireTargetsTargetingSystem)
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
        foreach (var group in targetedCells.CellGroups.Keys)
        {
            foreach (var pos in targetedCells.CellGroups[group])
                _battleMapView.HighlightCell(pos.x, pos.y, colors[group]);
        }
    }

    private void OnConfirmationUnlocked(IRequireTargetsTargetingSystem multiTargetSystem)
        => Debug.Log("Ability use ready.");
    private void OnConfirmationLocked(IRequireTargetsTargetingSystem multiTargetSystem)
        => Debug.Log("Ability use locked.");

    #region ToRemove
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_selectedAbility != null && _selectedAbility.AbilityData.TargetingSystem.IsConfirmAvailable)
            {
                var abilityName = _selectedAbility.AbilityData.View.Name;
                _selectedAbility.AbilityData.TargetingSystem.ConfirmTargeting();
                Debug.Log($"Ability «{abilityName}» has been used." % Colorize.Green);
            }
        }
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
    }
    #endregion
}
