using System;
using JetBrains.Annotations;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RoguelikeMap.UI.Characters
{
    public class DropZone : MonoBehaviour, IDropHandler
    {
        [CanBeNull] public CharacterCard CharacterCard { get; private set; }
        public bool IsEmpty => CharacterCard is null;
        public event Action<DropZone, CharacterCard> OnTrySelect;

        public void OnDrop(PointerEventData eventData)
        {
            if (!eventData.pointerDrag.gameObject.TryGetComponent<DraggableObject>(out var dragObject))
                return;
            if (dragObject is null)
                return;
            OnTrySelect?.Invoke(this, dragObject.DragObject);
        }

        public void Select(CharacterCard card)
        {
            if(CharacterCard is not null)
                Unselect();
            CharacterCard = card;
        }

        private void Unselect()
        {
            if (!CharacterCard.IsDestroyed())
                Destroy(CharacterCard.gameObject);
            CharacterCard = null;
        }

        public int TryGetCost()
        {
            if (IsEmpty)
                return -1;
            if (CharacterCard is not CharacterCardWithCost characterCardWithCost)
                return -1;
            return characterCardWithCost.Cost;
        }
    }
}