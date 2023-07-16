using DG.Tweening;
using OrderElimination.AbilitySystem;
using System.Linq;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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
        private BattleStatUIBar _tempArmorBar;
        [SerializeField]
        private HoldableButton _avatarButton;
        [SerializeField]
        private EffectsList _effectsList;
        [SerializeField]
        private Image _panelHighlightImage;

        [Header("Parameters")]
        [SerializeField]
        private float _highlightTime = 0.5f;
        [SerializeField]
        private Ease _highlightEase = Ease.Flash;
        [HideInInspector, SerializeField]
        private bool _isClickingAvatarAvailable;
        [HideInInspector, SerializeField]
        private bool _isHoldingAvatarAvailable;

        private Sequence _currentSequence;
        private BattleEntityView _currentEntityView;

        [ShowInInspector]
        public bool IsClickingAvatarAvailable
        {
            get => _isClickingAvatarAvailable;
            set
            {
                _isClickingAvatarAvailable = value;
                _avatarButton.ClickAvailable = value;
            }
        }

        [ShowInInspector]
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
                Logging.LogException(new System.ArgumentNullException());
            if (_currentEntityView != null)
                Unsubscribe(_currentEntityView.BattleEntity);
            _currentEntityView = entity;
            Subscribe(_currentEntityView.BattleEntity);
            _avatar.sprite = _currentEntityView.BattleIcon;
            IsClickingAvatarAvailable = true;
            UpdateStats(_currentEntityView, true);
            UpdateEffects();
            ShowInfo();
        }

        public void Highlight(Color highlightColor)
        {
            _currentSequence?.Complete();
            var appearTime = 0.1f;
            _currentSequence = DOTween.Sequence(this)
                .Append(_panelHighlightImage.DOColor(highlightColor, appearTime))
                .Insert(0, transform.DOScale(1.1f, appearTime))
                .Append(transform.DOScale(1, _highlightTime).SetEase(_highlightEase))
                .Append(_panelHighlightImage.DOColor(Color.white, 0.8f))
                .Play();
        }

        public void HideInfo()
        {
            var bars = new[] { _healthBar, _armorBar, _tempArmorBar };
            foreach (var bar in bars)
            {
                bar.SetNumberInstant(0);
                bar.SetFillInstant(0);
                bar.gameObject.SetActive(false);
            }

            _avatar.gameObject.SetActive(false);
            _avatarButton.gameObject.SetActive(false);
            IsClickingAvatarAvailable = false;
            _avatarButton.Clicked -= OnAvatarButtonPressed;
            _avatarButton.Holded -= OnAvatarButtonHolded;

            _effectsList.gameObject.SetActive(false);
        }

        public void ShowInfo()
        {
            var bars = new[] { _healthBar, _armorBar, _tempArmorBar };
            foreach (var bar in bars)
            {
                bar.gameObject.SetActive(true);
            }

            _avatar.gameObject.SetActive(true);
            _avatarButton.gameObject.SetActive(true);
            IsClickingAvatarAvailable = true;
            _avatarButton.Clicked += OnAvatarButtonPressed;
            _avatarButton.Holded += OnAvatarButtonHolded;

            _effectsList.gameObject.SetActive(true);
        }

        private void OnAvatarButtonHolded(HoldableButton avatarButton, float holdingTime)
            => OnAvatarButtonPressed(avatarButton);

        private void OnAvatarButtonPressed(HoldableButton avatarButton)
        {
            var characterDescriptionPanel =
                (CharacterDescriptionPanel) UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
            characterDescriptionPanel.UpdateCharacterDescription(_currentEntityView.BattleEntity);
        }

        private void Subscribe(AbilitySystemActor entity)
        {
            //entity.Damaged += OnDamaged;
            //entity.Healed += OnHealed;
            entity.BattleStats.LifeStatsChanged += OnLifeStatsChanged;
            entity.EffectAdded += OnEffectsUpdated;
            entity.EffectRemoved += OnEffectsUpdated;
            entity.BattleStats.StatsChanged += OnStatsChanged;
            entity.DisposedFromBattle += OnDisposedFromBattle;
        }

        private void Unsubscribe(AbilitySystemActor entity)
        {
            //entity.Damaged -= OnDamaged;
            //entity.Healed -= OnHealed;
            entity.BattleStats.LifeStatsChanged -= OnLifeStatsChanged;
            entity.EffectAdded -= OnEffectsUpdated;
            entity.EffectRemoved -= OnEffectsUpdated;
            entity.BattleStats.StatsChanged -= OnStatsChanged;
            entity.DisposedFromBattle -= OnDisposedFromBattle;
        }

        #region EntityEventHandlers
        private void OnStatsChanged(BattleStat stat) => UpdateStats(_currentEntityView, false);
        private void OnLifeStatsChanged(IBattleLifeStats stats) => UpdateStats(_currentEntityView, false);
        private void UpdateStats(BattleEntityView entityView, bool isInstant)
        {
            var stats = entityView.BattleEntity.BattleStats;
            var curHealth = stats.Health;
            var maxHealth = stats.MaxHealth.ModifiedValue;
            var pureArmor = stats.PureArmor;
            var totalArmor = stats.TotalArmor;
            var tempArmor = stats.TemporaryArmor;
            var maxArmor = stats.MaxArmor.ModifiedValue;

            SetBarNumber(_healthBar, curHealth, isInstant);
            SetBarFill(_healthBar, curHealth, maxHealth, isInstant);
            SetBarFill(_armorBar, pureArmor, maxArmor, isInstant);
            SetBarFill(_tempArmorBar, tempArmor, maxArmor, isInstant);
            if (tempArmor > 0)
            {
                _tempArmorBar.IsValueVisible = true;
                _armorBar.IsValueVisible = false;
                SetBarNumber(_tempArmorBar, totalArmor, isInstant);
            }
            else
            {
                _tempArmorBar.IsValueVisible = false;
                _armorBar.IsValueVisible = true;
                SetBarNumber(_armorBar, totalArmor, isInstant);
            }

            static void SetBarNumber(BattleStatUIBar bar, float number, bool instant)
            {
                if (instant)
                    bar.SetNumberInstant(number);
                else
                    bar.SetNumber(number);
            }

            static void SetBarFill(BattleStatUIBar bar, float value, float maxValue, bool instant)
            {
                var notNanValue = value == 0 ? value : value / maxValue;
                if (instant)
                    bar.SetFillInstant(notNanValue);
                else
                    bar.SetFill(notNanValue);
            }
        }
        private void OnEffectsUpdated(BattleEffect effect) => UpdateEffects();
        private void UpdateEffects() => _effectsList.UpdateEffects(_currentEntityView.BattleEntity.Effects);
        private void OnDisposedFromBattle(IBattleDisposable entity)
        {
            if (_currentEntityView.BattleEntity != entity)
                Logging.LogException( new System.Exception());
            Unsubscribe(_currentEntityView.BattleEntity);
            _currentSequence.Complete();
            _panelHighlightImage.DOComplete();
            transform.DOComplete();
        }
        #endregion
    }
}