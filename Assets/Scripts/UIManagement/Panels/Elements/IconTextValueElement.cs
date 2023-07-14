using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class IconTextValueElement: MonoBehaviour
    {
        [Title("Components")]
        [SerializeField]
        private Image _iconComponent;
        [SerializeField] 
        private TextMeshProUGUI _textComponent;
        [SerializeField] 
        private TextMeshProUGUI _valueComponent;

        [Title("Settings")]
        [ShowInInspector]
        public bool HasIcon
        {
            get => _iconComponent == null ? false : _iconComponent.gameObject.activeSelf;
            set
            {
                if (_iconComponent != null)
                    _iconComponent.gameObject.SetActive(value);
            }
        }
        [ShowInInspector]
        public bool HasText
        {
            get => _textComponent == null ? false : _textComponent.gameObject.activeSelf;
            set
            {
                if (_textComponent != null)
                    _textComponent.gameObject.SetActive(value);
            }
        }
        [ShowInInspector]
        public bool HasValue
        {
            get => _valueComponent == null ? false : _valueComponent.gameObject.activeSelf;
            set
            {
                if (_valueComponent != null)
                    _valueComponent.gameObject.SetActive(value);
            }
        }
        [ShowInInspector]
        public float IconSize
        {
            get => _iconComponent == null ? 0 : _iconComponent.rectTransform.sizeDelta.x;
            set
            {
                if (_iconComponent != null)
                {
                    _iconComponent.rectTransform.sizeDelta = Vector2.one * value;
                }
            }
        }
        [ShowInInspector]
        public Sprite Icon
        {
            get => _iconComponent == null ? null : _iconComponent.sprite;
            set
            {
                if (_iconComponent != null)
                    _iconComponent.sprite = value;
            }
        }
        [ShowInInspector]
        public string Text
        {
            get => _textComponent == null ? null : _textComponent.text;
            set
            {
                if (_textComponent != null)
                    _textComponent.text = value;
            }
        }
        [ShowInInspector]
        public string Value
        {
            get => _valueComponent == null ? null : _valueComponent.text;
            set 
            {
                if (_valueComponent != null)
                    _valueComponent.text = value;
            }
        }

        public event Action<IconTextValueElement> Destroyed;

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
