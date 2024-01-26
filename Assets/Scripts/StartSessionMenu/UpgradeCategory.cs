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

        private int _progressCount;

        public float StartUpgradeWidthPart = 0.5f;
        public float EndUpgradeWidthPart = 0.9f;
        public float AnimationTime = 0.5f;
        public float ShiftPart = 50f;

        public int PercentInPart = 5;
        private int MaxProgressCount = 6;
        

        private int StartCostOfUpgrade = 200;
        public int CostOfUpgrade { get; private set; } = 200;
        public int ProgressCount
        {
            get => _progressCount;
            set
            {
                if (value < 0) value = 0;
                if (value > MaxProgressCount) value = MaxProgressCount;
                var prevValue = _progressCount;
                if (prevValue != value)
                {
                    _progressCount = value;
                    CostOfUpgrade = GetUpgradePrice(ProgressCount + 1);
                    VisualUpgrade();
                }
            }
        }

        public event Action<UpgradeCategory> OnUpgrade;

        private void Start()
        {
            _costText.text = $"{CostOfUpgrade}$";
            _upgradeButton.onClick.AddListener(ClickOnUpgradeButton);
        }

        private void ClickOnUpgradeButton()
        {
            OnUpgrade?.Invoke(this);
        }
        
        public bool CanUpgrade(int availableMoney)
        {
            if (availableMoney < CostOfUpgrade)
                return false;
            if (ProgressCount == MaxProgressCount - 1)
                return false;
            return true;
        }

        public void Upgrade(int availableMoney)
        {
            if (!CanUpgrade(availableMoney))
                throw new InvalidOperationException();
            ProgressCount++;
            CostOfUpgrade = GetUpgradePrice(ProgressCount + 1);
            VisualUpgrade();
        }

        private void VisualUpgrade()
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
            
            _costText.text = $"{CostOfUpgrade}$";
            _percentText.text = $"{(ProgressCount + 1) * PercentInPart}%";
        }

        private int GetUpgradePrice(int targetProgressCount)
        {
            var price = targetProgressCount * StartCostOfUpgrade;
            //Debug.Log($"UpgradeCost({targetProgressCount}) = {price}");
            return price;
        }
    }
}