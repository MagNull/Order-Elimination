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

    public void BindAbilityButtons(BattleMapView mapView, AbilityPanel abilityPanel)
    {
        mapView.CellClicked += OnCellClicked(abilityPanel);
    }

    private Action<CellView> OnCellClicked(AbilityPanel abilityPanel)
    {
        return cell =>
        {
            if (!CheckCell(cell) || abilityPanel.AbilityCasing)
                return;
            var characterView = cell.Model.Objects.First(obj => obj is BattleCharacter).View.GameObject
                .GetComponent<BattleCharacterView>();
            _selectedCharacterView?.Deselect();
            _selectedCharacterView = characterView;
            _selectedCharacterView.Select();
            abilityPanel.AssignAbilities(characterView.ActiveAbilitiesView, characterView.PassiveAbilitiesView);
            abilityPanel.SelectFirstAvailableAbility();
        };
    }

    private bool CheckCell(CellView cell)
    {
        return cell.Model.Contains(obj => obj is BattleCharacter, out var character) &&
               character.Type == BattleObjectType.Ally &&
               BattleSimulation.BattleState == BattleState.PlayerTurn &&
               !character.View.GameObject.GetComponent<BattleCharacterView>().IsSelected;
    }

    public void OnDisable(BattleMapView mapView, AbilityPanel abilityPanel)
    {
        mapView.CellClicked -= OnCellClicked(abilityPanel);
    }
}