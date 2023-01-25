using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using OrderElimination.BM;
using UIManagement;
using UnityEngine;

public class AbilityViewBinder 
{
    private BattleCharacterView _selectedCharacterView;

    public void BindAbilityButtons(BattleMapView mapView, AbilityPanel abilityPanel, BattleObjectSide currentTurn)
    {
        mapView.CellClicked += OnCellClicked(abilityPanel, currentTurn);
    }

    private Action<CellView> OnCellClicked(AbilityPanel abilityPanel, BattleObjectSide currentTurn)
    {
        return cell =>
        {
            if (cell.Model.GetObject() is NullBattleObject ||
                !cell.Model.GetObject().View.GameObject ||
                !cell.Model.GetObject().View.GameObject.TryGetComponent(out BattleCharacterView characterView)
                || characterView.Model.Side != BattleObjectSide.Ally
                || BattleSimulation.CurrentTurn != BattleObjectSide.Ally
                || characterView.IsSelected
                || abilityPanel.AbilityCasing)
                return;
            _selectedCharacterView?.Deselect();
            _selectedCharacterView = characterView;
            _selectedCharacterView.Select();
            abilityPanel.AssignAbilities(characterView.ActiveAbilitiesView, characterView.PassiveAbilitiesView);
            //TODO(Сано): Автовыбор перемещения независимо от порядка
            abilityPanel.SelectFirstAvailableAbility();
        };
    }

    public void OnDisable(BattleMapView mapView, AbilityPanel abilityPanel)
    {
        mapView.CellClicked -= OnCellClicked(abilityPanel, BattleObjectSide.Ally);
    }
}
