using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using OrderElimination.BM;
using UIManagement;

public class AbilityViewBinder 
{
    private BattleCharacterView _selectedCharacterView;

    public void BindAbilityButtons(BattleMapView mapView, AbilityPanel abilityPanel, BattleObjectSide currentTurn)
    {
        mapView.CellClicked += cell =>
        {
            if (cell.Model.GetObject() is NullBattleObject ||
                !cell.Model.GetObject().View.GameObject.TryGetComponent(out BattleCharacterView characterView)
                || characterView.Model.Side != BattleObjectSide.Ally
                || currentTurn != BattleObjectSide.Ally
                || characterView.IsSelected
                || abilityPanel.AbilityCasing)
                return;
            _selectedCharacterView?.Deselect();
            _selectedCharacterView = characterView;
            _selectedCharacterView.Select();
            abilityPanel.AssignAbilities(characterView.ActiveAbilitiesView, characterView.PassiveAbilitiesView);
            //TODO(Сано): Автовыбор перемещения независимо от порядка
            abilityPanel.Select(characterView.ActiveAbilitiesView.FirstOrDefault(a => a.CanCast));
        };
    }
}
