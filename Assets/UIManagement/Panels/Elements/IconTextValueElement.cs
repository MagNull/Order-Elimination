using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class IconTextValueElement: MonoBehaviour
    {
        [SerializeField] private Image _iconComponent;
        [SerializeField] private TextMeshProUGUI _textComponent;
        [SerializeField] private TextMeshProUGUI _valueComponent;
        public event Action<IconTextValueElement> Destroyed;

        [ShowInInspector]
        public bool HasIcon
        {
            get => _iconComponent == null ? false : _iconComponent.gameObject.activeSelf;
            set => _iconComponent.gameObject.SetActive(value);
        }
        [ShowInInspector]
        public bool HasText
        {
            get => _textComponent == null ? false : _textComponent.gameObject.activeSelf;
            set => _textComponent.gameObject.SetActive(value);
        }
        [ShowInInspector]
        public bool HasValue
        {
            get => _valueComponent == null ? false : _valueComponent.gameObject.activeSelf;
            set => _valueComponent.gameObject.SetActive(value);
        }

        private float _iconSize = 54;
        [ShowInInspector]
        public float IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                _iconComponent.rectTransform.sizeDelta = Vector2.one * _iconSize;
            }
        }
        [ShowInInspector]
        public Sprite Icon
        {
            get => _iconComponent == null ? null : _iconComponent.sprite;
            set => _iconComponent.sprite = value;
        }
        [ShowInInspector]
        public string Text
        {
            get => _textComponent == null ? null : _textComponent.text;
            set => _textComponent.text = value;
        }
        [ShowInInspector]
        public string Value
        {
            get => _valueComponent == null ? null : _valueComponent.text;
            set => _valueComponent.text = value;
        }

        public void UpdateInfo(Sprite icon = null, string text = null, string value = null)
        {
            Icon = icon;
            Text = text;
            Value = value;
        }

        public void UpdateInfo(Sprite icon = null, string text = null, float value = 0) => UpdateInfo(icon, text, value.ToString());
        public void UpdateInfo(Sprite icon = null, string text = null, int value = 0) => UpdateInfo(icon, text, value.ToString());

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
