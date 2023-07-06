using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using OrderElimination;
using System;
using OrderElimination.Infrastructure;

namespace UIManagement.Elements
{
    public class BattleStatUIBar : MonoBehaviour
    {
        private Tween _currentTween;
        private float _currentScaledValue;

        [Header("Components")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _barImage;
        [SerializeField] private TextMeshProUGUI _textValueComponent;
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnColorChange))]
        public Color BarColor = Color.red;
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnIconChange))] 
        public Sprite Icon;

        [Header("Parameters")]
        [SerializeField] public float TweeningTime = 0.4f;
        [SerializeField] public Ease ValueChangeEase = Ease.OutBounce;
        [SerializeField] public bool TweenNumbers;
        [SerializeField] public bool RoundNumbers = true;
        [ShowIf(nameof(RoundNumbers))]
        [SerializeField] public RoundingOption RoundingMode = RoundingOption.Math;
        public float FillAmount => _barImage.fillAmount;

        [Button]
        public void SetValue(float currentValue, float minValue, float maxValue)
        {
            var scaledValue = (currentValue - minValue);
            if (scaledValue != 0)
                scaledValue /= (maxValue - minValue);
            if (float.IsNaN(scaledValue)) //scaledValue > 1 || scaledValue < 0 || 
            {
                Logging.Log("Normalized value: " + scaledValue + '\n' +
                          "Max value: " + maxValue + '\n' +
                          "Min value: " + minValue + '\n');
                Logging.LogException( new System.InvalidOperationException("Value is NaN"));
            }
            if (_currentScaledValue == scaledValue)
                return;

            _currentScaledValue = scaledValue;
            if (_currentTween != null)
                _currentTween.Complete();
            if (RoundNumbers)
                currentValue = MathExtensions.Round(currentValue, RoundingMode);
            _textValueComponent.text = currentValue.ToString();
            _currentTween = DOTween
                .To(GetFillAmount, SetFillAmount, scaledValue, TweeningTime)
                .SetEase(ValueChangeEase).
                OnComplete(() => _currentTween = null);

            float GetFillAmount() => FillAmount;

            void SetFillAmount(float amount)
            {
                if (TweenNumbers)
                {
                    var value = Mathf.Lerp(minValue, maxValue, amount);
                    if (RoundNumbers)
                        value = MathExtensions.Round(value, RoundingMode);
                    _textValueComponent.text = value.ToString();
                }
                _barImage.fillAmount = amount;
            }
        }

        private void OnColorChange(Color newBarColor) => _barImage.color = newBarColor;

        private void OnIconChange(Sprite newIcon) => _iconImage.sprite = newIcon;
    } 
}
