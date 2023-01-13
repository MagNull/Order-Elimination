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
        private BattleCharacterView currentCharacterView;

        [SerializeField]
        private bool _isClickingAvatarAvailable;
        public bool IsClickingAvatarAvailable
        {
            get => _isClickingAvatarAvailable;
            set
            {
                _isClickingAvatarAvailable = value;
                _avatarButton.ClickAvailable = value;
            }
        }

        [SerializeField]
        private bool _isHoldingAvatarAvailable;
        public bool IsHoldingAvatarAvailable
        {
            get => _isHoldingAvatarAvailable;
            set
            {
                _isHoldingAvatarAvailable = value;
                _avatarButton.HoldAvailable = value;
            }
        }

        public void UpdateCharacterInfo(BattleCharacterView characterView)
        {
            print($"Данные для персонажа {characterView.CharacterName} обновлены");
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
            characterView.Model.Died += OnCharacterDied;
            var stats = characterView.Model.Stats;
            _healthBar.SetValue(stats.Health, 0, stats.UnmodifiedHealth);
            _armorBar.SetValue(stats.Armor, 0, stats.UnmodifiedArmor);

            print($"Бафов на персонаже {characterView.CharacterName}: {characterView.Model.CurrentBuffEffects.Count}");
            print($"Временных эффектов на персонаже {characterView.CharacterName}: {characterView.Model.CurrentTickEffects.Count}");
            print($"Текущих эффектов на персонаже {characterView.CharacterName}: {characterView.Model.IncomingTickEffects.Count}");
            foreach (var e in characterView.Model.CurrentBuffEffects)
            {
                var effectView = e.GetEffectView();
                var effectName = effectView.GetEffectName();
                print($"На персонажа {characterView.CharacterName} наложен бафф {effectName}");
            }
            foreach (var e in characterView.Model.CurrentTickEffects)
            {
                var effectView = e.GetEffectView();
                var effectName = effectView.GetEffectName();
                print($"На персонажа {characterView.CharacterName} наложен эффект {effectName}");
            }
            foreach (var e in characterView.Model.IncomingTickEffects)
            {
                var effectView = e.GetEffectView();
                var effectName = effectView.GetEffectName();
                print($"На персонажа {characterView.CharacterName} наложен временный эффект {effectName}");
            }
            _avatar.sprite = characterView.Icon;
        }

        private void OnCharacterEffectAdded(ITickEffect effect) => UpdateCharacterInfo(currentCharacterView);

        private void OnCharacterDamaged(TakeDamageInfo damageInfo) => UpdateCharacterInfo(currentCharacterView);

        private void OnCharacterDied(BattleCharacter character) => UpdateCharacterInfo(null);

        private void OnAvatarButtonHolded(HoldableButton avatarButton, float holdingTime)
            => OnAvatarButtonPressed(avatarButton);

        private void OnAvatarButtonPressed(HoldableButton avatarButton)
        {
            var characterDescriptionPanel = (CharacterDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.CharacterDetails);
            characterDescriptionPanel.UpdateCharacterDescription(currentCharacterView);
        }

        public void HideInfo()
        {
            _avatar.gameObject.SetActive(false);
            _healthBar.gameObject.SetActive(false);
            _armorBar.gameObject.SetActive(false);
            _avatarButton.gameObject.SetActive(false);
            _avatarButton.Clicked -= OnAvatarButtonPressed;
            _avatarButton.Holded -= OnAvatarButtonHolded;
        }

        public void ShowInfo()
        {
            _avatar.gameObject.SetActive(true);
            _healthBar.gameObject.SetActive(true);
            _armorBar.gameObject.SetActive(true);
            _avatarButton.gameObject.SetActive(true);
            _avatarButton.Clicked += OnAvatarButtonPressed;
            _avatarButton.Holded += OnAvatarButtonHolded;
        }
    } 
}
