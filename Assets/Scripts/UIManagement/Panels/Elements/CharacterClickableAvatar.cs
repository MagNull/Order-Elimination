using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderElimination;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UIManagement.Elements
{
    public class CharacterClickableAvatar: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private Image _characterAvatar;
        [SerializeField] private HoldableButton _avatarButton;

        public bool IsClickable
        {
            get => _avatarButton.ClickAvailable;
            set => _avatarButton.ClickAvailable = value;
        }

        public event Action<CharacterClickableAvatar> Clicked;
        public event Action<CharacterClickableAvatar> Holded;

        public void UpdateCharacterInfo(string name, Sprite avatarIcon)
        {
            RemoveCharacterInfo();
            _characterName.text = name;
            _characterAvatar.sprite = avatarIcon;
            _avatarButton.Clicked += OnAvatarButtonClicked;
            _avatarButton.Holded += OnAvatarButtonHolded;
        }

        public void RemoveCharacterInfo()
        {
            _characterName.text = null;
            _characterAvatar.sprite = null;
            _avatarButton.Clicked -= OnAvatarButtonClicked;
            _avatarButton.Holded -= OnAvatarButtonHolded;
        }

        private void OnAvatarButtonClicked(HoldableButton button) => Clicked?.Invoke(this);

        private void OnAvatarButtonHolded(HoldableButton button, float holdTime) => Holded?.Invoke(this);
    }
}
