using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    //TODO: THIS IS VIEW-TYPE CLASS. ALL UPGRADE FUNCTIONALITY MUST NOT BE HERE
    public class UpgradeCategory : MonoBehaviour
    {
        [SerializeField]
        private List<Image> _progressBar;
        [SerializeField]
        private Button _upgradeButton;
        [SerializeField]
        private TextMeshProUGUI _costText;
        [SerializeField]
        private TextMeshProUGUI _percentText;

        public float StartUpgradeWidthPart = 0.5f;
        public float EndUpgradeWidthPart = 0.9f;
        public float AnimationTime = 0.5f;
        public float ShiftPart = 50f;

        private int _progressCount;
        private int _costOfUpgrade;
        private int _increaseAmount;

        public int CostOfUpgrade
        {
            get => _costOfUpgrade;
            set
            {
                _costOfUpgrade = value;
                _costText.text = $"{_costOfUpgrade}$";
            }
        }

        public int IncreaseAmount
        {
            get => _increaseAmount;
            set
            {
                _increaseAmount = value;
                _percentText.text = $"+{_increaseAmount}%";
            }
        }

        public int ProgressCount
        {
            get => _progressCount;
            set
            {
                if (value < 0) value = 0;
                if (value > 5) value = 5;
                var prevValue = _progressCount;
                if (prevValue != value)
                {
                    _progressCount = value;
                    VisualUpdate();
                }
            }
        }

        public event Action<UpgradeCategory> UpgradeButtonClicked;

        private void Start()
        {
            CostOfUpgrade = -1;
            IncreaseAmount = -1;
            ProgressCount = 0;
            _upgradeButton.onClick.AddListener(ClickOnUpgradeButton);
        }

        private void ClickOnUpgradeButton()
        {
            UpgradeButtonClicked?.Invoke(this);
        }

        private void VisualUpdate()
        {
            var firstPart = _progressBar[ProgressCount - 1];
            firstPart.DOColor(Color.yellow, AnimationTime);
            var firstPartTransform = firstPart.transform;
            firstPartTransform.DOComplete();
            firstPartTransform.DOScaleX(StartUpgradeWidthPart, AnimationTime);
            firstPartTransform.DOMoveX(firstPartTransform.position.x - ShiftPart, AnimationTime);

            var secondPartTransform = _progressBar[ProgressCount].transform;
            secondPartTransform.transform.DOScaleX(EndUpgradeWidthPart, AnimationTime);
            secondPartTransform.transform.DOMoveX(secondPartTransform.position.x - ShiftPart, AnimationTime);
        }
    }
}