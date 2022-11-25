using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using CharacterAbility.AbilityEffects;
using UnityEngine;
using OrderElimination.BattleMap;

[Serializable]
public class AbilityViewBinder
{
    [SerializeField]
    private AbilityInfo _movementInfo;
    private BattleCharacterView _selectedCharacterView;

    public void BindAbilityButtons(BattleMapView mapView, AbilityButton[] abilityButtons, BattleObjectSide currentTurn)
    {
        mapView.CellClicked += cell =>
        {
            if (cell.Model.GetObject() is NullBattleObject ||
                !cell.Model.GetObject().View.TryGetComponent(out BattleCharacterView characterView)
                || characterView.Model.Side != BattleObjectSide.Ally
                || currentTurn != BattleObjectSide.Ally
                || characterView.Selected)
                return;

            _selectedCharacterView?.Deselect();
            _selectedCharacterView = characterView;
            _selectedCharacterView.Select();
            for (var i = 0; i < characterView.AbilityViews.Length; i++)
            {
                abilityButtons[i].CancelAbilityCast();
                abilityButtons[i].SetAbility(characterView.AbilityViews[i]);
            }

            abilityButtons.First(abilityButton => abilityButton.GetAbilityInfo() == _movementInfo).OnClicked();
        };
    }
}