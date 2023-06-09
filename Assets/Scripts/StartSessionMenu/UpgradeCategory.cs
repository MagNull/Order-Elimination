using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    public class UpgradeCategory : MonoBehaviour
    {
        public float StartUpgradeWidthPart = 0.5f;
        public float EndUpgradeWidthPart = 0.9f;
        public float AnimationTime = 0.5f;
        public float ShiftPart = 50f;
        public int PercentInPart = 5;
        private int MaxProgressCount = 6;
        
        [SerializeField]
        private List<Image> _progressBar;
        [SerializeField]
        private Button _upgradeButton;
        [SerializeField]
        private TextMeshProUGUI _costText;
        [SerializeField]
        private TextMeshProUGUI _percentText;
        
        public event Action<UpgradeCategory> OnUpgrade;

        private int StartCostOfUpgrade = 200;
        public int CostOfUpgrade { get; private set; } = 200;
        public int ProgressCount { get; private set; } = 0;

        private void Start()
        {
            _costText.text = $"{CostOfUpgrade}$";
            _upgradeButton.onClick.AddListener(ClickOnUpgradeButton);
        }

        private void ClickOnUpgradeButton()
        {
            OnUpgrade?.Invoke(this);
        }
        
        public bool TryUpgrade(int availableMoney)
        {
            if (availableMoney < CostOfUpgrade)
                return false;
            if (ProgressCount == MaxProgressCount - 1)
                return false;
            CostOfUpgrade += StartCostOfUpgrade;
            ProgressCount++;
            VisualUpgrade();
            return true;
        }

        private void VisualUpgrade()
        {
            var firstPart = _progressBar[ProgressCount - 1];
            var firstPartTransform = firstPart.transform;
            firstPartTransform.DOScaleX(StartUpgradeWidthPart, AnimationTime);
            firstPartTransform.DOMoveX(firstPartTransform.position.x - ShiftPart, AnimationTime);
            
            var secondPartTransform = _progressBar[ProgressCount].transform;
            secondPartTransform.transform.DOScaleX(EndUpgradeWidthPart, AnimationTime);
            secondPartTransform.transform.DOMoveX(secondPartTransform.position.x - ShiftPart, AnimationTime);
            
            _costText.text = $"{CostOfUpgrade}$";
            _percentText.text = $"{(ProgressCount + 1) * PercentInPart}%";
        }
    }
}