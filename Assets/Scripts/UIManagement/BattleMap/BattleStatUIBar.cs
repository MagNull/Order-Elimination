using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using OrderElimination;
using System;
using OrderElimination.Infrastructure;
using Sirenix.Serialization;
using static UnityEngine.Rendering.DebugUI;

namespace UIManagement.Elements
{
    public class BattleStatUIBar : SerializedMonoBehaviour
    {
        private Tween _currentNumberTween;
        private Tween _currentFillTween;
        private float _currentNumber = float.NaN;
        private float _targetNumber = float.NaN;
        private float _targetFillAmount = float.NaN;

        #region Components
        [TitleGroup("Components")]
        [SerializeField, PropertyOrder(-10)] 
        private Image _iconImage;
        [SerializeField, PropertyOrder(-9)]
        private Image _barImage;
        [SerializeField, PropertyOrder(-8)]
        private TextMeshProUGUI _textValueComponent;
        #endregion Components

        #region Visuals
        [BoxGroup("Properties/Visuals")]
        [PropertyOrder(-3)]
        [ShowInInspector, PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public Sprite Icon
        {
            get => _iconImage.sprite;
            set => _iconImage.sprite = value;
        }

        [BoxGroup("Properties/Visuals")]
        [ShowInInspector]
        public Color BarColor
        {
            get => _barImage.color;
            set => _barImage.color = value;
        }

        [BoxGroup("Properties/Visuals")]
        [BoxGroup("Properties/Visuals/Components Visibility", ShowLabel = false)]
        [ShowInInspector]
        public bool IsIconVisible
        {
            get => _iconImage != null ? _iconImage.gameObject.activeSelf : false;
            set => _iconImage.gameObject.SetActive(value);
        }

        [BoxGroup("Properties/Visuals")]
        [BoxGroup("Properties/Visuals/Components Visibility")]
        [ShowInInspector]
        public bool IsValueVisible
        {
            get => _textValueComponent != null ? _textValueComponent.gameObject.activeSelf : false;
            set => _textValueComponent.gameObject.SetActive(value);
        }

        [BoxGroup("Properties/Visuals")]
        [BoxGroup("Properties/Visuals/Components Visibility")]
        [ShowInInspector]
        public bool IsFillBarVisible
        {
            get => _barImage != null ? _barImage.gameObject.activeSelf : false;
            set => _barImage.gameObject.SetActive(value);
        }

        [BoxGroup("Properties/Visuals")]
        [BoxGroup("Properties/Visuals/Tweening", ShowLabel = false)]
        [OdinSerialize]
        public bool TweenValues { get; set; }

        [BoxGroup("Properties/Visuals")]
        [BoxGroup("Properties/Visuals/Tweening")]
        [ShowIf(nameof(TweenValues))]
        [OdinSerialize]
        public float TweeningTime { get; set; } = 0.4f;

        [BoxGroup("Properties/Visuals")]
        [BoxGroup("Properties/Visuals/Tweening")]
        [ShowIf(nameof(TweenValues))]
        [OdinSerialize]
        public Ease ValueChangeEase { get; set; } = Ease.OutBounce;

        [TitleGroup("Properties")]
        [BoxGroup("Properties/Rounding"), PropertyOrder(-2)]
        [OdinSerialize]
        public bool RoundNumbers { get; set; } = true;

        [BoxGroup("Properties/Rounding")]
        [ShowIf(nameof(RoundNumbers))]
        [OdinSerialize]
        public RoundingOption RoundingMode { get; set; } = RoundingOption.Math;
        #endregion

        [Button]
        public void SetFillAndNumber(float currentValue, float minValue, float maxValue)
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
            SetNumber(currentValue);
            SetFill(scaledEndValue);
        }

        public void SetNumber(float value)
        {
            if (float.IsNaN(value) || !float.IsFinite(value))
                throw new ArgumentException();
            if (value == _targetNumber)
                return;
            _targetNumber = value;
            if (float.IsNaN(_currentNumber))
                _currentNumber = value;
            if (_currentNumberTween != null)
                _currentNumberTween.Complete(false);
            if (TweenValues)
            {
                _currentNumberTween = DOTween
                    .To(GetValue, SetValue, value, TweeningTime)
                    .SetEase(ValueChangeEase)
                    .OnComplete(() => _currentNumberTween = null);
            }
            else
                SetValue(value);

            float GetValue() => _currentNumber;

            void SetValue(float value)
            {
                _currentNumber = value;
                if (RoundNumbers)
                    value = MathExtensions.Round(value, RoundingMode);
                _textValueComponent.text = value.ToString();
            }
        }

        public void SetFill(float fillAmount)
        {
            if (float.IsNaN(fillAmount) || !float.IsFinite(fillAmount))
                throw new ArgumentException();
            if (fillAmount == _targetFillAmount)
                return;
            var initialFill = _barImage.fillAmount;
            var endFill = Mathf.Clamp01(fillAmount);
            _targetFillAmount = endFill;
            if (_currentFillTween != null)
                _currentFillTween.Complete();
            if (TweenValues)
            {
                _currentFillTween = DOTween
                    .To(GetFillAmount, SetFillAmount, endFill, TweeningTime)
                    .SetEase(ValueChangeEase)
                    .OnComplete(() => _currentFillTween = null);
            }
            else
                SetFillAmount(endFill);

            float GetFillAmount() => _barImage.fillAmount;

            void SetFillAmount(float normalizedValue) => _barImage.fillAmount = normalizedValue;
        }

        public void SetNumberInstant(float value)
        {
            if (float.IsNaN(value) || !float.IsFinite(value))
                throw new ArgumentException();
            _currentNumberTween.Complete(false);
            _currentNumber = value;
            _targetNumber = value;
            if (RoundNumbers)
                value = MathExtensions.Round(value, RoundingMode);
            _textValueComponent.text = value.ToString();
        }

        public void SetFillInstant(float fillAmount)
        {
            if (float.IsNaN(fillAmount) || !float.IsFinite(fillAmount))
                throw new ArgumentException();
            if (_currentFillTween != null)
                _currentFillTween.Complete();
            _targetFillAmount = fillAmount;
            _barImage.fillAmount = fillAmount;
        }
    } 
}
