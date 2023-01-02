using OrderElimination.BattleMap;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class CharacterBattleStatsPanel : MonoBehaviour
    {
        [SerializeField] private Image _avatar;
        [SerializeField] private BattleStatUIBar _healthBar;
        [SerializeField] private BattleStatUIBar _armorBar;
        [SerializeField] private HoldableButton _avatarButton;
        [SerializeField] private UIController _panelController;
        private BattleCharacterView currentCharacterView;

        public void UpdateCharacterInfo(BattleCharacterView characterView)
        {
            if (currentCharacterView != null)
            {
                currentCharacterView.Model.Damaged -= OnCharacterDamaged;
                currentCharacterView.Model.EffectAdded -= OnCharacterEffectAdded;
                currentCharacterView.Model.Died -= OnCharacterDied;
            }
            currentCharacterView = characterView;
            if (characterView == null)
            {
                HideInfo();
                return;
            }
            ShowInfo();
            characterView.Model.Damaged += OnCharacterDamaged;
            characterView.Model.EffectAdded += OnCharacterEffectAdded;
            currentCharacterView.Model.Died += OnCharacterDied;
            var stats = characterView.Model.Stats;
            _healthBar.SetValue(stats.Health, 0, stats.UnmodifiedHealth);
            _armorBar.SetValue(stats.Armor, 0, stats.UnmodifiedArmor);
            _avatar.sprite = characterView.Icon;
        }

        private void OnCharacterEffectAdded(ITickEffect effect) => UpdateCharacterInfo(currentCharacterView);

        private void OnCharacterDamaged(TakeDamageInfo damageInfo) => UpdateCharacterInfo(currentCharacterView);

        private void OnCharacterDied(BattleCharacter character) => UpdateCharacterInfo(null);

        public void Bind(BattleMapView mapView, BattleObjectSide currentTurn)
        {
            //mapView.CellClicked += cell =>
            //{
            //    if (cell.Model.GetObject() is NullBattleObject ||
            //        !cell.Model.GetObject().View.TryGetComponent(out BattleCharacterView characterView)
            //        || characterView.Model.Side != BattleObjectSide.Ally
            //        || currentTurn != BattleObjectSide.Ally)
            //        return;
            //    UpdateCharacterInfo(characterView);
            //};

            BattleCharacterView.Selected += UpdateCharacterInfo;
            BattleCharacterView.Deselected += OnCharacterDeselected;
        }

        private void Start()
        {
            HideInfo();
        }

        private void OnAvatarButtonPressed(HoldableButton avatarButton)
        {
            var characterDescriptionPanel = (CharacterDescriptionPanel)_panelController.OpenPanel(PanelType.CharacterDetails);
            characterDescriptionPanel.UpdateCharacterDescription(currentCharacterView);
        }

        private void OnCharacterDeselected(BattleCharacterView character)
        {
            HideInfo();
        }

        private void HideInfo()
        {
            _avatar.gameObject.SetActive(false);
            _healthBar.gameObject.SetActive(false);
            _armorBar.gameObject.SetActive(false);
            _avatarButton.gameObject.SetActive(false);
            _avatarButton.Clicked -= OnAvatarButtonPressed;
        }

        private void ShowInfo()
        {
            _avatar.gameObject.SetActive(true);
            _healthBar.gameObject.SetActive(true);
            _armorBar.gameObject.SetActive(true);
            _avatarButton.gameObject.SetActive(true);
            _avatarButton.Clicked += OnAvatarButtonPressed;
        }
    } 
}
