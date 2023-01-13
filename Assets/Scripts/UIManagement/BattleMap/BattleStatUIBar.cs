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
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnColorChange))]
        public Color BarColor = Color.red;
        [SerializeField, ShowInInspector, OnValueChanged(nameof(OnIconChange))] 
        public Sprite Icon;
        [SerializeField] public Ease ValueChangeEase = Ease.OutBounce;
        public float FillAmount => _barImage.fillAmount;

        [Button]
        public void SetValue(float currentValue, float minValue, float maxValue)
        {
            _textValueComponent.text = currentValue.ToString();
            var normalizedValue = (currentValue - minValue) / (maxValue - minValue);
            if (normalizedValue > 1 || normalizedValue < 0 || float.IsNaN(normalizedValue))
                throw new System.InvalidOperationException();
            DOTween.To(GetFillAmount, SetFillAmount, normalizedValue, 0.4f).SetEase(ValueChangeEase);

            float GetFillAmount() => FillAmount;

            void SetFillAmount(float amount) => _barImage.fillAmount = amount;
        }

        private void OnColorChange(Color newBarColor) => _barImage.color = newBarColor;

        private void OnIconChange(Sprite newIcon) => _iconImage.sprite = newIcon;
    } 
}
