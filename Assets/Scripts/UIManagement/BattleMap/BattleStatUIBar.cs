using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using OrderElimination;
using System;
using OrderElimination.Infrastructure;
using static UnityEngine.Rendering.DebugUI;

namespace UIManagement.Elements
{
    public class BattleStatUIBar : MonoBehaviour
    {
        private Tween _currentNumberTween;
        private Tween _currentFillTween;
        private float _currentValue = 0;
        private float _targetValue = float.NaN;

        [Header("Components")]
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _barImage;
        [SerializeField] private TextMeshProUGUI _textValueComponent;
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnColorChange))]
        public Color BarColor = Color.red;
        [field: SerializeField]
        public Color DefaultTextColor { get; set; }
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnIconChange))] 
        public Sprite Icon;

        [Header("Parameters")]
        [SerializeField] public float TweeningTime = 0.4f;
        [SerializeField] public Ease ValueChangeEase = Ease.OutBounce;
        [SerializeField] public bool TweenNumbers;
        [SerializeField] public bool RoundNumbers = true;
        [ShowIf(nameof(RoundNumbers))]
        [SerializeField] public RoundingOption RoundingMode = RoundingOption.Math;
        public float ActualFillAmount => _barImage.fillAmount;
        public float TargetFillAmount { get; private set; }

        [Button]
        public void SetFill(float currentValue, float minValue, float maxValue)
        {
            var scaledEndValue = (currentValue - minValue);
            if (scaledEndValue != 0)
                scaledEndValue /= (maxValue - minValue);
            if (float.IsNaN(scaledEndValue)) //scaledValue > 1 || scaledValue < 0 || 
            {
                Logging.Log("Normalized value: " + scaledEndValue + '\n' +
                          "Max value: " + maxValue + '\n' +
                          "Min value: " + minValue + '\n');
                Logging.LogException(new InvalidOperationException("Value is NaN"));
            }
            //
            SetValue(currentValue);
            SetFill(scaledEndValue);
        }

        public void SetValue(float value)
        {
            if (value == _targetValue)
                return;
            _targetValue = value;
            var duration = TweenNumbers ? TweeningTime : 0;
            if (_currentNumberTween != null)
                _currentNumberTween.Complete(false);
            _currentNumberTween = DOTween
                .To(GetValue, SetValue, value, duration)
                .SetEase(ValueChangeEase)
                .OnComplete(() => _currentNumberTween = null);

            float GetValue() => _currentValue;

            void SetValue(float value)
            {
                _currentValue = value;
                if (RoundNumbers)
                    value = MathExtensions.Round(value, RoundingMode);
                _textValueComponent.text = value.ToString();
            }
        }

        public void SetFill(float fillAmount)
        {
            if (fillAmount == TargetFillAmount)
                return;
            var initialFill = ActualFillAmount;
            var endFill = Mathf.Clamp01(fillAmount);
            TargetFillAmount = endFill;
            var duration = TweenNumbers ? TweeningTime : 0;
            if (_currentFillTween != null)
                _currentFillTween.Complete();
            _currentFillTween = DOTween
                .To(GetFillAmount, SetFillAmount, endFill, duration)
                .SetEase(ValueChangeEase)
                .OnComplete(() => _currentFillTween = null);

            float GetFillAmount() => ActualFillAmount;

            void SetFillAmount(float normalizedValue) => _barImage.fillAmount = normalizedValue;
        }

        [Button]
        public void SetTextColor(Color color)
        {
            _textValueComponent.color = color;
        }

        public void ResetTextColor()
        {
            _textValueComponent.color = DefaultTextColor;
        }

        private void OnColorChange(Color newBarColor) => _barImage.color = newBarColor;

        private void OnIconChange(Sprite newIcon) => _iconImage.sprite = newIcon;
    } 
}
