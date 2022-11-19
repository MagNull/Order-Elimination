using System;
using System.Collections;
using System.Collections.Generic;
using CharacterAbility;
using UnityEngine;
using OrderElimination.BattleMap;

public class AbilityViewBinder 
{
    private BattleCharacterView _selectedCharacterView;

    public void BindAbilityButtons(BattleMapView mapView, AbilityButton[] abilityButtons, BattleObjectSide currentTurn)
    {
        mapView.CellClicked += cell =>
        {
            if (cell.Model.GetObject() is NullBattleObject ||
                !cell.Model.GetObject().GetView().TryGetComponent(out BattleCharacterView characterView)
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
            //TODO(Сано): Автовыбор перемещения независимо от порядка
            abilityButtons[0].OnClicked();
        };
    }
}
