using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

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

        private Tween _currentTween;

        [Button]
        public async UniTask SetValue(float currentValue, float minValue, float maxValue)
        {
            _textValueComponent.text = currentValue.ToString();
            var scaledValue = (currentValue - minValue);
            if (scaledValue != 0)
                scaledValue /= (maxValue - minValue);
            if (float.IsNaN(scaledValue)) //scaledValue > 1 || scaledValue < 0 || 
            {
                Debug.Log("Normalized value: " + scaledValue + '\n' +
                          "Max value: " + maxValue + '\n' +
                          "Min value: " + minValue + '\n');
                throw new System.InvalidOperationException("Value is NaN");
            }
            if (_currentTween != null)
                _currentTween.Complete();
            _currentTween = DOTween.To(GetFillAmount, SetFillAmount, scaledValue, 0.4f).SetEase(ValueChangeEase);
            await _currentTween.AsyncWaitForCompletion();

            float GetFillAmount() => FillAmount;

            void SetFillAmount(float amount) => _barImage.fillAmount = amount;
        }

        private void OnColorChange(Color newBarColor) => _barImage.color = newBarColor;

        private void OnIconChange(Sprite newIcon) => _iconImage.sprite = newIcon;
    } 
}
