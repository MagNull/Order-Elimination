using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace UIManagement.Elements
{
    public class BattleStatUIBar : MonoBehaviour
    {
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _barImage;
        [SerializeField] private TextMeshProUGUI _textValueComponent;
        [SerializeField] private RectMask2D _barMask;
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnColorChange))]
        public Color BarColor = Color.red;
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnIconChange))] 
        public Sprite Icon;
        [SerializeField] public Ease ValueChangeEase = Ease.OutBounce;

        private float FillMaxMaskValue => 0;
        private float FillMinMaskValue => ((RectTransform)_barMask.transform).rect.width;

        [Button]
        public void SetValue(float currentValue, float minValue, float maxValue)
        {
            var normalizedValue = (currentValue - minValue) / (maxValue - minValue);
            var rightPadding = FillMinMaskValue + (FillMaxMaskValue - FillMinMaskValue) * normalizedValue;
            _textValueComponent.text = currentValue.ToString();
            DOTween.To(() => _barMask.padding.z, SetMaskPaddingRight, rightPadding, 0.4f).SetEase(ValueChangeEase);
        }

        
        private void SetMaskPaddingRight(float paddingValue)
        {
            var maskPadding = _barMask.padding;
            maskPadding.z = paddingValue;
            _barMask.padding = maskPadding;
        }

        private void OnColorChange(Color newBarColor) => _barImage.color = newBarColor;

        private void OnIconChange(Sprite newIcon) => _iconImage.sprite = newIcon;
    } 
}
