using Sirenix.OdinInspector;
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
        [SerializeField] private CharacterAvatarsList _characterList;
        public override PanelType PanelType => PanelType.SquadList;
        
        [Button]
        public void UpdateSquadInfo(List<Character> squadCharacters)
        {
            _characterList.Clear();
            _characterList.Populate(squadCharacters.ToArray());
            _characterList.ElementHolded -= OnCharacterAvatarHolded;
            _characterList.ElementHolded += OnCharacterAvatarHolded;
        }

        private void OnCharacterAvatarHolded(CharacterClickableAvatar characterAvatar)
        {
            var characterPanel = (CharacterDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            characterPanel.UpdateCharacterDescription(characterAvatar.CurrentCharacterInfo);
        }
    }
}
