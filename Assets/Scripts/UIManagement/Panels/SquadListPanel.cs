using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIManagement.Debugging;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;
using Character = OrderElimination.Character;

namespace UIManagement
{
    public class SquadListPanel : UIPanel
    {
        [SerializeField] private CharacterList _characterList;
        public override PanelType PanelType => PanelType.SquadList;

        public void UpdateSquadListPanel(List<BattleCharacterView> squadCharacters)
        {
            Debug.Log("1");
            _characterList.HasExperienceRecieved = false;
            _characterList.HasMaintenanceCost = true;
            _characterList.HasParameters = true;
            _characterList.Add(squadCharacters.ToArray());
        }
        
        public void UpdateSquadListPanel(List<Character> squadCharacters)
        {
            _characterList.Clear();
            _characterList.HasExperienceRecieved = false;
            _characterList.HasMaintenanceCost = false;
            _characterList.HasParameters = false;
            _characterList.Add(squadCharacters.ToArray());
        }
    }
}
