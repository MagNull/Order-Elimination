using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class CooldownTimer : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField]
        private TextMeshProUGUI _cooldownValueText;
        [SerializeField]
        private Image _cooldownImage;

        [field: Header("Settings")]
        [field: SerializeField]
        public float ValueChangeDuration { get; set; }
        [field: SerializeField]
        private bool HideValuesBelowOne { get; set; }
        [field: SerializeField]
        private bool IgnoreSameValues { get; set; }

        private int _currentValue = -1;

        private void Reset()
        {
            _cooldownValueText = GetComponentInChildren<TextMeshProUGUI>();
            _cooldownImage = GetComponentInChildren<Image>();
        }

        private void Awake()
        {
            SetValue(0);
        }

        [Button]
        public void SetValue(int value) => SetValue(value, ValueChangeDuration, null);

        public void SetValue(int value, float transitionTime) => SetValue(value, transitionTime, null);

        public void SetValue(int value, Color valueColor)
            => SetValue(value, ValueChangeDuration, valueColor);

        public void SetValue(int value, Color valueColor, float transitionTime)
            => SetValue(value, transitionTime, valueColor);

        private void SetValue(int value, float transitionTime, Color? valueColor)
        {
            if (IgnoreSameValues && _currentValue == value)
                return;
            _currentValue = value;
            var rotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            this.DOComplete();
            DOTween.Sequence(this)
                .Append(_cooldownValueText.rectTransform.DOLocalRotateQuaternion(rotation, transitionTime / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => OnValueChange(value, valueColor)))
                .Append(_cooldownValueText.rectTransform.DOLocalRotateQuaternion(Quaternion.identity, transitionTime / 2)
                    .SetEase(Ease.Linear))
                .Play();
        }

        private void OnValueChange(int value, Color? numberColor)
        {
            _cooldownValueText.text = value.ToString();
            if (numberColor.HasValue)
                _cooldownValueText.color = numberColor.Value;
            if (value <= 0 && HideValuesBelowOne)
            {
                _cooldownValueText.gameObject.SetActive(false);
                _cooldownImage.gameObject.SetActive(false);
                this.DOKill();
            }
            else
            {
                _cooldownValueText.gameObject.SetActive(true);
                _cooldownImage.gameObject.SetActive(true);
            }
        }
    } 
}
