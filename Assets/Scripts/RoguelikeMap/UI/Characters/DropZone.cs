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
        private DraggableObject _lastDragObject;
        [CanBeNull] public CharacterCard CharacterCard { get; private set; }
        public bool IsEmpty => CharacterCard is null;
        public event Action<DropZone, CharacterCard> OnTrySelect;

        public void OnDrop(PointerEventData eventData)
        {
            if (!eventData.pointerDrag.gameObject.TryGetComponent<DraggableObject>(out var dragObject))
                return;
            if (dragObject is null)
                return;
            _lastDragObject = dragObject.DragObject.GetComponent<DraggableObject>();
            if (_lastDragObject.IsCopy
                && dragObject.DragObject.IsSelected)
                return;
            OnTrySelect?.Invoke(this, dragObject.DragObject);
        }

        public void Select(CharacterCard card)
        {
            if(CharacterCard is not null)
                Unselect();
            CharacterCard = card;
            _lastDragObject.Dropped += Unselect;
        }

        private void Unselect(CharacterCard card = null)
        {
            if (!CharacterCard.IsDestroyed())
                Destroy(CharacterCard.gameObject);
            CharacterCard = null;
        }

        public int? GetCost()
        {
            if (IsEmpty || CharacterCard is not CharacterCardWithCost characterCardWithCost)
                return null;
            return characterCardWithCost.Cost;
        }
    }
}