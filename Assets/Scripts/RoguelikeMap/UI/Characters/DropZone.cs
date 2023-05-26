using System;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap.UI.Characters
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        public event Action<DropZone, CharacterCard> OnTrySelect;
        
        public void OnDrop(PointerEventData eventData)
        {
            eventData.pointerDrag.gameObject.TryGetComponent<CharacterCard>(out var characterCard);
            if (characterCard is null)
                return;
            OnTrySelect?.Invoke(this, characterCard);
        }
    }
}