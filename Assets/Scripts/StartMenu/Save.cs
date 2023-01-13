using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination.Start
{
    public class Save : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _deleteButton;
        private Color _activeImageColor = Color.white;
        private Color _defaultImageColor = Color.gray;
        public bool IsActive { get; private set; }
        public event Action DeleteSave;

        public void SetText(string text)
        {
            _text.text = text;
            // _deleteButton.interactable = IsActive && !IsEmpySave();
            if(!_deleteButton.IsDestroyed())
                _deleteButton.gameObject.SetActive(IsActive && !IsEmptySave());
        }

        public bool IsEmptySave() => _text.text == "";

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            _image.color = isActive
                ? _activeImageColor
                : _defaultImageColor;
            _deleteButton.gameObject.SetActive(isActive && !IsEmptySave());
        }

        public void DeleteButtonClicked()
        {
            DeleteSave?.Invoke();
            SetText("");
        }
    }
}