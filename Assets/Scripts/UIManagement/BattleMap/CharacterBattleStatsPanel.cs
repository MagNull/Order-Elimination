using DG.Tweening;
using OrderElimination.AbilitySystem;
using OrderElimination.BM;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
        private BattleEntityView _currentEntityView;

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

        public void UpdateEntityInfo(BattleEntityView entity)
        {
            if (entity == null)
                throw new System.ArgumentNullException();
            if (_currentEntityView != null)
            {
                _currentEntityView.BattleEntity.Damaged -= OnDamaged;
                _currentEntityView.BattleEntity.Healed -= OnHealed;
                _currentEntityView.BattleEntity.EffectAdded -= OnEffectsUpdated;
                _currentEntityView.BattleEntity.EffectRemoved -= OnEffectsUpdated;
                _currentEntityView.BattleEntity.BattleStats.StatsChanged -= OnStatsChanged;
            }
            _currentEntityView = entity;
            _currentEntityView.BattleEntity.Damaged += OnDamaged;
            _currentEntityView.BattleEntity.Healed += OnHealed;
            _currentEntityView.BattleEntity.EffectAdded += OnEffectsUpdated;
            _currentEntityView.BattleEntity.EffectRemoved += OnEffectsUpdated;
            _currentEntityView.BattleEntity.BattleStats.StatsChanged += OnStatsChanged;
            _avatar.sprite = _currentEntityView.BattleIcon;
            UpdateStats();
            UpdateEffects();

            void OnStatsChanged(BattleStat stat) => UpdateStats();
            void OnDamaged(DealtDamageInfo damage) => UpdateStats();
            void OnHealed(HealRecoveryInfo heal) => UpdateStats();
            void UpdateStats()
            {
                var stats = _currentEntityView.BattleEntity.LifeStats;

                //Round visual numbers
                var curHealth = Mathf.RoundToInt(stats.Health);
                var maxHealth = Mathf.RoundToInt(stats.MaxHealth.ModifiedValue);
                var curArmor = Mathf.RoundToInt(stats.TotalArmor);
                var maxArmor = Mathf.RoundToInt(stats.MaxArmor.ModifiedValue);
                //Round visual numbers

                _healthBar.SetValue(curHealth, 0, maxHealth);
                _armorBar.SetValue(curArmor, 0, maxArmor);
            }
            void OnEffectsUpdated(BattleEffect effect) => UpdateEffects();
            void UpdateEffects() => _effectsList.UpdateEffects(_currentEntityView.BattleEntity.Effects);
        }

        public void Highlight(Color highlightColor)
        {
            _panelHighlightImage.color = highlightColor;
            transform.localScale = Vector3.one * 1.1f;
            _highlightTweeners.Add(transform.DOScale(1, _highlightTime / 2.5f).SetEase(_highlightEase));

            //_highlightTweeners.Add(_panelHighlightImage.DOBlendableColor(Color.white, _highlightTime)
            //    .SetEase(_highlightEase));
        }

        public void FinishHighlightAnimation()
        {
            foreach (var t in _highlightTweeners)
                t.Complete();
            _panelHighlightImage.color = Color.white;
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

        private void OnAvatarButtonHolded(HoldableButton avatarButton, float holdingTime)
            => OnAvatarButtonPressed(avatarButton);

        private void OnAvatarButtonPressed(HoldableButton avatarButton)
        {
            var characterDescriptionPanel =
                (CharacterDescriptionPanel) UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            //characterDescriptionPanel.UpdateCharacterDescription(_currentCharacterView);
        }
    }
}