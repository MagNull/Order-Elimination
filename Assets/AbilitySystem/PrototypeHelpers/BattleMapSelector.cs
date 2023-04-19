using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIManagement;
using UIManagement.Elements;
using UnityEngine;
using VContainer;

public class BattleMapSelector : MonoBehaviour
{
    [SerializeField]
    private AbilityPanel _abilityPanel;
    [SerializeField]
    private CharacterBattleStatsPanel _characterBattleStatsPanel;

    private BattleMapView _battleMapView;
    private CharacterArrangeDirector _characterArrangeDirector;
    private IBattleContext _battleContext;
    private SelectorMode _mode;

    private CellView _lastClickedCell;
    private int currentLoopIndex;

    [Inject]
    private void Construct(BattleMapView battleMapView, CharacterArrangeDirector characterArrangeDirector)
    {
        _battleMapView = battleMapView;
        _battleMapView.CellClicked += OnCellClicked;
        _characterArrangeDirector = characterArrangeDirector;
    }

    private void OnCellClicked(CellView cellView)
    {
        if (_lastClickedCell != cellView)
            currentLoopIndex = 0;
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
            if (entities.Length == 0)
                return;
            currentLoopIndex %= entities.Length;
            var selectedEntity = entities[currentLoopIndex];
            currentLoopIndex++;

            var view = _characterArrangeDirector.GetViewByEntity(selectedEntity);
            _abilityPanel.AssignAbilities(selectedEntity.ActiveAbilities.ToArray(), new AbilityRunner[0]);
            _characterBattleStatsPanel.UpdateEntityInfo(view);
            _characterBattleStatsPanel.ShowInfo();

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

            print($"{selectedEntity.EntityType} {view.Name} selected");
        }
        else if (_mode == SelectorMode.SelectingTargetCell)
        {

        }
    }
}

public enum SelectorMode
{
    SelectingUnit,
    SelectingTargetCell
}
