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
    public partial class CharacterClickableAvatar: MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _characterName;
        [SerializeField] private Image _characterAvatar;
        [SerializeField] private HoldableButton _avatarButton;
        public Character CurrentCharacterInfo { get; private set; }
        public IBattleCharacterInfo currentBattleCharacterInfo { get; private set; }

        public event Action<CharacterClickableAvatar> Clicked;
        public event Action<CharacterClickableAvatar> Holded;

        public void UpdateCharacterInfo(Character character)
        {
            if (character == null)
            {
                throw new InvalidOperationException();
            }
            RemoveCharacterInfo();
            CurrentCharacterInfo = character;
            _characterName.text = character.Name;
            _characterAvatar.sprite = character.Avatar;
            _avatarButton.Clicked += OnAvatarButtonClicked;
            _avatarButton.Holded += OnAvatarButtonHolded;
        }

        public void UpdateCharacterInfo(IBattleCharacterInfo characterInfo)
        {
            if (characterInfo == null)
            {
                throw new InvalidOperationException();
            }
            RemoveCharacterInfo();
            currentBattleCharacterInfo = characterInfo;
            _characterName.text = characterInfo.GetName();
            _characterAvatar.sprite = characterInfo.GetViewAvatar();
            _avatarButton.Clicked += OnAvatarButtonClicked;
            _avatarButton.Holded += OnAvatarButtonHolded;
        }

        public void RemoveCharacterInfo()
        {
            CurrentCharacterInfo = null;
            _characterName.text = null;
            _characterAvatar.sprite = null;
            _avatarButton.Clicked -= OnAvatarButtonClicked;
            _avatarButton.Holded -= OnAvatarButtonHolded;
        }

        private void OnAvatarButtonClicked(HoldableButton button) => Clicked?.Invoke(this);

        private void OnAvatarButtonHolded(HoldableButton button, float holdTime)
        {
            Holded?.Invoke(this);
        }
    }

    partial class CharacterClickableAvatar
    {
        public event Action<CharacterClickableAvatar> Destroyed;

        private void OnDestroy()
        {
            Destroyed?.Invoke(this);
        }
    }
}
