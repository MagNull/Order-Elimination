using DG.Tweening;
using OrderElimination.AbilitySystem;
using OrderElimination.BM;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class CharacterBattleStatsPanel : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private Image _avatar;
        [SerializeField]
        private BattleStatUIBar _healthBar;
        [SerializeField]
        private BattleStatUIBar _armorBar;
        [SerializeField]
        private HoldableButton _avatarButton;
        [SerializeField]
        private EffectsList _effectsList;
        [SerializeField]
        private Image _panelHighlightImage;
        private BattleCharacterView _currentCharacterView;

        [Header("Parameters")]
        [SerializeField]
        private float _highlightTime = 0.7f;
        [SerializeField]
        private Ease _highlightEase = Ease.Flash;
        [SerializeField]
        private bool _isClickingAvatarAvailable;
        [SerializeField]
        private bool _isHoldingAvatarAvailable;

        private List<Tweener> _highlightTweeners = new ();

        public bool IsClickingAvatarAvailable
        {
            get => _isClickingAvatarAvailable;
            set
            {
                _isClickingAvatarAvailable = value;
                _avatarButton.ClickAvailable = value;
            }
        }

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
            if (characterView == null)
            {
                HideInfo();
                return;
            }

            var battleCharacter = (BattleCharacter) characterView.Model;
            if (_currentCharacterView != null)
            {
                battleCharacter.Damaged -= OnCharacterDamaged;
                battleCharacter.EffectAdded -= OnCharacterEffectAdded;
                battleCharacter.EffectRemoved -= OnCharacterEffectRemoved;
                battleCharacter.Died -= OnCharacterDied;
            }

            _currentCharacterView = characterView;
            if (characterView == null)
            {
                HideInfo();
                return;
            }

            ShowInfo();
            battleCharacter.Damaged += OnCharacterDamaged;
            battleCharacter.EffectAdded += OnCharacterEffectAdded;
            battleCharacter.EffectRemoved += OnCharacterEffectRemoved;
            battleCharacter.Died += OnCharacterDied;
            var stats = characterView.Model.Stats;
            _healthBar.SetValue(stats.Health, 0, stats.UnmodifiedHealth);
            _armorBar.SetValue(stats.Armor + stats.AdditionalArmor, 0,
                stats.UnmodifiedArmor + stats.AdditionalArmor);

            var effects = battleCharacter.CurrentBuffEffects
                .Concat(battleCharacter.CurrentTickEffects)
                .Concat(battleCharacter.IncomingTickEffects)
                .ToArray();
            //.Where()
            _effectsList.UpdateEffects(effects);
            _avatar.sprite = characterView.Icon;
        }

        public void UpdateEntityInfo(BattleEntityView entity)
        {
            _avatar.sprite = entity.BattleIcon;
            var stats = entity.BattleEntity.LifeStats;
            _healthBar.SetValue(stats.Health, 0, stats.MaxHealth.ModifiedValue);
            _armorBar.SetValue(stats.TotalArmor, 0, stats.MaxArmor.ModifiedValue);
            //entity.BattleEntity.Damaged += 
        }

        public void Highlight(Color highlightColor)
        {
            _panelHighlightImage.color = highlightColor;
            transform.localScale = Vector3.one * 1.1f;
            _highlightTweeners.Add(transform.DOScale(1, _highlightTime / 2.5f).SetEase(_highlightEase));

            //_highlightTweeners.Add(_panelHighlightImage.DOBlendableColor(Color.white, _highlightTime)
            //    .SetEase(_highlightEase));
        }

        public void KillHighlightAnimation()
        {
            foreach (var t in _highlightTweeners)
                t.Complete();
            _panelHighlightImage.color = Color.white;
        }

        private void OnCharacterEffectAdded(ITickEffect effect) => UpdateCharacterInfo(_currentCharacterView);
        private void OnCharacterEffectRemoved(ITickEffect effect) => UpdateCharacterInfo(_currentCharacterView);

        private void OnCharacterDamaged(TakeDamageInfo damageInfo) => UpdateCharacterInfo(_currentCharacterView);

        private void OnCharacterDied(BattleCharacter character) => UpdateCharacterInfo(null);

        private void OnAvatarButtonHolded(HoldableButton avatarButton, float holdingTime)
            => OnAvatarButtonPressed(avatarButton);

        private void OnAvatarButtonPressed(HoldableButton avatarButton)
        {
            var characterDescriptionPanel =
                (CharacterDescriptionPanel) UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            characterDescriptionPanel.UpdateCharacterDescription(_currentCharacterView);
        }

        public void HideInfo()
        {
            _avatar.gameObject.SetActive(false);
            _healthBar.gameObject.SetActive(false);
            _armorBar.gameObject.SetActive(false);
            _avatarButton.gameObject.SetActive(false);
            _effectsList.gameObject.SetActive(false);
            _avatarButton.Clicked -= OnAvatarButtonPressed;
            _avatarButton.Holded -= OnAvatarButtonHolded;
        }

        public void ShowInfo()
        {
            _avatar.gameObject.SetActive(true);
            _healthBar.gameObject.SetActive(true);
            _armorBar.gameObject.SetActive(true);
            _avatarButton.gameObject.SetActive(true);
            _effectsList.gameObject.SetActive(true);
            _avatarButton.Clicked += OnAvatarButtonPressed;
            _avatarButton.Holded += OnAvatarButtonHolded;
        }
    }
}