using System;
using GameInventory.Items;
using GameInventory.Views;
using OrderElimination;
using RoguelikeMap.UI.Characters;
using StartSessionMenu.ChooseCharacter.CharacterCard;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameInventory
{
    public class UsableItemsView : PlayerInventoryView
    {
        [SerializeField]
        private SquadMembersPanel _squadMembersPanel;

        private bool _clickChecking;

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            if (cell.Item is not IUsable)
                return;
            base.OnCellAdded(cell);
        }

        private void Update()
        {
            if(_clickChecking)
                ResetItemUseOnClick();
        }

        private void ResetItemUseOnClick()
        {
            if (!Input.GetMouseButtonDown(0))
                return;
            
            var selectedObject = EventSystem.current.currentSelectedGameObject;
            if(selectedObject == null || !selectedObject.GetComponent<CharacterCard>())
                ResetItemUse();
        }

        private void OnEnable()
        {
            CellClicked += OnCellClicked;
        }

        private void OnDisable()
        {
            CellClicked -= OnCellClicked;
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            var usableItem = cell.Item as IUsable;
            if (usableItem == null)
                Logging.LogException(new ArgumentException("Item is not usable"), this);

            foreach (var characterCard in _squadMembersPanel.CharacterCards)
            {
                if(!usableItem.CheckConditionToUse(characterCard.Character))
                    continue;
                
                _clickChecking = true;
                characterCard.EnableHighlight();
                characterCard.SetSpecialClickEvent(() =>
                {
                    usableItem.Use(characterCard.Character);
                    ResetItemUse();
                    _clickChecking = false;
                });
            }
        }

        private void ResetItemUse()
        {
            foreach (var card in _squadMembersPanel.CharacterCards)
            {
                card.ResetSpecialClickEvent();
                card.DisableHighlight();
            }
        }
    }
}