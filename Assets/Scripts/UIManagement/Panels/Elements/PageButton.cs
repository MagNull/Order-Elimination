using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    [ExecuteInEditMode]
    public class PageButton : MonoBehaviour
    {
        private bool _isInitialized = false;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _buttonText;
        public Button Button => _button;

        public string Text
        {
            get => _buttonText.text;
            set => _buttonText.text = value;
        }

        public TextMeshProUGUI TextComponent => _buttonText;

        public event Action<PageButton> Clicked;
        public event Action<PageButton> Destroyed;

        private void Initialize()
        {
            if (_isInitialized)
                return;
            _button.onClick.AddListener(OnPageButtonPressed);
            _isInitialized = true;
        }

        private void OnPageButtonPressed()
        {
            Clicked?.Invoke(this);
        }

        private void OnEnable()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
