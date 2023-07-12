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
            UpdateStats(_currentEntityView);
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
            _avatar.gameObject.SetActive(false);
            _healthBar.gameObject.SetActive(false);
            _armorBar.gameObject.SetActive(false);
            _avatarButton.gameObject.SetActive(false);
            _effectsList.gameObject.SetActive(false);
            IsClickingAvatarAvailable = false;
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
            IsClickingAvatarAvailable = true;
            _avatarButton.Clicked += OnAvatarButtonPressed;
            _avatarButton.Holded += OnAvatarButtonHolded;
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
        private void OnStatsChanged(BattleStat stat) => UpdateStats(_currentEntityView);
        private void OnDamaged(DealtDamageInfo damage) => UpdateStats(_currentEntityView);
        private void OnHealed(DealtRecoveryInfo heal) => UpdateStats(_currentEntityView);
        private void OnLifeStatsChanged(IBattleLifeStats stats) => UpdateStats(_currentEntityView);
        private void UpdateStats(BattleEntityView entityView)
        {
            var stats = entityView.BattleEntity.BattleStats;
            var curHealth = stats.Health;
            var maxHealth = stats.MaxHealth.ModifiedValue;
            var curArmor = stats.TotalArmor;
            var maxArmor = stats.MaxArmor.ModifiedValue;

            _healthBar.SetValue(curHealth, 0, maxHealth);
            _armorBar.SetValue(curArmor, 0, maxArmor);
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