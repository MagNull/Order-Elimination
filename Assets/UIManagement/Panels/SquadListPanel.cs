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

namespace UIManagement
{
    public class SquadListPanel : UIPanel
    {
        [SerializeField] private CharacterList _characterList;
        public override PanelType PanelType => PanelType.SquadList;

        public void UpdateSquadListPanel(List<BattleCharacterView> squadCharacters)
        {
            _characterList.HasExperienceRecieved = false;
            _characterList.HasMaintenanceCost = true;
            _characterList.HasParameters = true;
            _characterList.Add(squadCharacters.ToArray());
        }

        //ToRemove
        private void Awake()
        {
            UpdateSquadListPanel(new ExplorationResult().SquadCharacters);
        }
    }
}
