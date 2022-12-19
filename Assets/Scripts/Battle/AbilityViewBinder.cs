using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using OrderElimination.BattleMap;
using UIManagement;

public class AbilityViewBinder 
{
    private BattleCharacterView _selectedCharacterView;

    public void BindAbilityButtons(BattleMapView mapView, AbilityPanel abilityPanel, BattleObjectSide currentTurn)
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
            abilityPanel.AssignAbilities(characterView.ActiveAbilitiesView, characterView.PassiveAbilitiesView);
            //TODO(Сано): Автовыбор перемещения независимо от порядка
            abilityPanel.Select(characterView.ActiveAbilitiesView.First(a => a.CanCast));
        };
    }
}
