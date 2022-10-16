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
    public class IconTextValueElement: MonoBehaviour
    {
        [SerializeField] private Image _iconComponent;
        [SerializeField] private TextMeshProUGUI _textComponent;
        [SerializeField] private TextMeshProUGUI _valueComponent;

        public bool HasIcon
        {
            get => _iconComponent.gameObject.activeSelf;
            set => _iconComponent.gameObject.SetActive(value);
        }
        public bool HasText
        {
            get => _textComponent.gameObject.activeSelf;
            set => _textComponent.gameObject.SetActive(value);
        }
        public bool HasValue
        {
            get => _valueComponent.gameObject.activeSelf;
            set => _valueComponent.gameObject.SetActive(value);
        }

        private float _iconSize = 54;
        public float IconSize
        {
            get => _iconSize;
            set
            {
                _iconSize = value;
                _iconComponent.rectTransform.sizeDelta = Vector2.one * _iconSize;
            }
        }

        public Sprite Icon
        {
            get => _iconComponent.sprite;
            set => _iconComponent.sprite = value;
        }

        public string Text
        {
            get => _textComponent.text;
            set => _textComponent.text = value;
        }

        public string Value
        {
            get => _valueComponent.text;
            set => _valueComponent.text = value;
        }
    }
}
