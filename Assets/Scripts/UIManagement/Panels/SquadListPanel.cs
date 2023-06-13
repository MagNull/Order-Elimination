using Sirenix.OdinInspector;
using System.Collections.Generic;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using CharacterTemplate = OrderElimination.CharacterTemplate;

namespace UIManagement
{
    public class SquadListPanel : UIPanel
    {
        [SerializeField] private CharacterAvatarsList _characterList;
        public override PanelType PanelType => PanelType.SquadList;
        
        [Button]
        public void UpdateSquadInfo(List<CharacterTemplate> squadCharacters)
        {
            _characterList.Clear();
            _characterList.Populate(squadCharacters.ToArray());
            _characterList.ElementHolded -= OnCharacterAvatarHolded;
            _characterList.ElementHolded += OnCharacterAvatarHolded;
        }
        
        [Button]
        public void UpdateSquadInfo(List<IBattleCharacterInfo> squadCharacters)
        {
            _characterList.Clear();
            _characterList.Populate(squadCharacters.ToArray());
            _characterList.ElementHolded -= OnCharacterAvatarHolded;
            _characterList.ElementHolded += OnCharacterAvatarHolded;
        }

        private void OnCharacterAvatarHolded(CharacterClickableAvatar characterAvatar)
        {
            var upgradeTransaction = new CharacterUpgradeTransaction(characterAvatar.CurrentCharacterInfo);
            var characterPanel = (CharacterUpgradePanel)UIController.SceneInstance.OpenPanel(PanelType.CharacterUpgradable);
            characterPanel.UpdateCharacterUpgradeDescription(upgradeTransaction);
        }
    }
}
