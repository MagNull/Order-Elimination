using System;
using System.Collections;
using System.Collections.Generic;
using CharacterAbility;
using OrderElimination.BattleMap;

public class AbilityViewBinder 
{
    private BattleCharacterView _selectedCharacterView;

    public void BindAbilityButtons(BattleMapView mapView, AbilityButton[] abilityButtons, BattleObjectSide currentTurn)
    {
        mapView.CellClicked += cell =>
        {
            if (cell.Model.GetObject() is NullBattleObject ||
                !cell.Model.GetObject().View.TryGetComponent(out BattleCharacterView characterView)
                || characterView.Model.Side != BattleObjectSide.Ally
                || currentTurn != BattleObjectSide.Ally
                || characterView.IsSelected)
                return;

            _selectedCharacterView?.Deselect();
            _selectedCharacterView = characterView;
            _selectedCharacterView.Select();
            for (var i = 0; i < characterView.ActiveAbilitiesView.Length; i++)
            {
                abilityButtons[i].CancelAbilityCast();
                abilityButtons[i].SetAbility(characterView.ActiveAbilitiesView[i]);
            }
            //TODO(Сано): Автовыбор перемещения независимо от порядка
            abilityButtons[0].OnClick();
        };
    }
}
