using System;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap.UI.Characters
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        public event Action<DropZone, CharacterCard> OnTrySelect;
        public bool IsSelected { get; private set; } = false;
        
        public void OnDrop(PointerEventData eventData)
        {
            eventData.pointerDrag.gameObject.TryGetComponent<CharacterCard>(out var characterCard);
            if (characterCard is null)
                return;
            OnTrySelect?.Invoke(this, characterCard);
        }

        public void Select()
        {
            IsSelected = !IsSelected;
        }
    }
}