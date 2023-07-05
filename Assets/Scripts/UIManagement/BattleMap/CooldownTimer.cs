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
        public void SetValue(int value)
        {
            SetValue(value, ValueChangeDuration);
        }

        public void SetValue(int value, float setDuration)
        {
            if (IgnoreSameValues && _currentValue == value)
                return;
            _currentValue = value;
            var rotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
            this.DOComplete();
            DOTween.Sequence(this)
                .Append(_cooldownValueText.rectTransform.DOLocalRotateQuaternion(rotation, setDuration / 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => OnValueChange(value)))
                .Append(_cooldownValueText.rectTransform.DOLocalRotateQuaternion(Quaternion.identity, setDuration / 2)
                    .SetEase(Ease.Linear))
                .Play();
        }

        private void OnValueChange(int value)
        {
            _cooldownValueText.text = value.ToString();
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
