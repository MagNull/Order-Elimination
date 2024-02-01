using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    //TODO: THIS IS VIEW-TYPE CLASS. ALL UPGRADE FUNCTIONALITY MUST NOT BE HERE
    public class UpgradeCategory : MonoBehaviour
    {
        [SerializeField]
        private SectionalProgressBar _progressBar;
        [SerializeField]
        private Button _upgradeButton;
        [SerializeField]
        private TextMeshProUGUI _costText;
        [SerializeField]
        private TextMeshProUGUI _percentText;

        private float _unfocusedScale = 1;
        private int _costOfUpgrade;
        private int _increaseAmount;
        private int _progressLevel;

        public float FocusedScale = 2f;
        public float AnimationTime = 0.5f;

        [BoxGroup("Parameters")]
        [ShowInInspector]
        public int CostOfUpgrade
        {
            get => _costOfUpgrade;
            set
            {
                _costOfUpgrade = value;
                _costText.text = $"{_costOfUpgrade}$";
            }
        }

        [BoxGroup("Parameters")]
        [ShowInInspector]
        public int IncreaseAmount
        {
            get => _increaseAmount;
            set
            {
                _increaseAmount = value;
                _percentText.text = $"+{_increaseAmount}%";
            }
        }

        [BoxGroup("Parameters")]
        [ShowInInspector]
        public int ProgressLevel
        {
            get => _progressLevel;
            set
            {
                if (value < 0) value = 0;
                if (value > MaxUpgradeLevel) value = MaxUpgradeLevel;
                var prevValue = _progressLevel;
                if (prevValue != value)
                {
                    _progressLevel = value;
                    VisualUpdate();
                }
            }
        }

        [BoxGroup("Parameters")]
        [ShowInInspector]
        public int MaxUpgradeLevel
        {
            get => _progressBar != null ? _progressBar.SectionsCount : -1;
            set
            {
                if (value < 0) value = 0;
                _progressBar.SectionsCount = value;
                VisualUpdate();
            }
        }

        public event Action<UpgradeCategory> UpgradeButtonClicked;

        private void Awake()
        {
            CostOfUpgrade = -1;
            IncreaseAmount = -1;
            ProgressLevel = 0;
            _upgradeButton.onClick.AddListener(ClickOnUpgradeButton);
        }

        private void ClickOnUpgradeButton()
        {
            UpgradeButtonClicked?.Invoke(this);
        }

        private void VisualUpdate()
        {
            var sections = _progressBar.GetSections();
            for (var i = 0; i < sections.Count; i++)
            {
                var section = sections[i];
                section.TryGetComponent<Graphic>(out var graphic);
                section.DOKill(false);
                var color = Color.white;
                if (i < ProgressLevel)
                {
                    section.DOScaleX(_unfocusedScale, AnimationTime)
                        .OnUpdate(_progressBar.RecalculateLayout);
                    color = Color.yellow;
                }
                else if (i == ProgressLevel)
                {
                    section.DOScaleX(FocusedScale, AnimationTime)
                        .OnUpdate(_progressBar.RecalculateLayout);
                }
                else
                {
                    section.DOScaleX(_unfocusedScale, AnimationTime)
                        .OnUpdate(_progressBar.RecalculateLayout);
                }
                if (graphic != null)
                {
                    graphic.DOKill(false);
                    graphic.DOColor(color, AnimationTime);
                }
            }
        }
    }
}