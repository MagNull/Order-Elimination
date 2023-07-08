using RoguelikeMap.UI.Characters;
using UnityEngine;
using Utils;

namespace Inventory_Items
{
    public class UsableItemsView : PlayerInventoryView
    {
        [SerializeField]
        private SquadMembersPanel _squadMembersPanel;

        [SerializeField]
        private ClickingArea _cancelClickingArea;

        public override void OnCellAdded(IReadOnlyCell cell)
        {
            if (cell.Item is not IUsable)
                return;
            base.OnCellAdded(cell);
        }

        private void OnEnable()
        {
            _cancelClickingArea.PointerDown += () => Debug.Log("Cancel click");
            CellClicked += OnCellClicked;
        }

        private void OnDisable()
        {
            CellClicked -= OnCellClicked;
        }

        private void OnCellClicked(IReadOnlyCell cell)
        {
            var usableItem = cell.Item as IUsable;
            foreach (var characterCard in _squadMembersPanel.CharacterCards)
            {
                characterCard.SetSpecialClickEvent(() =>
                {
                    usableItem.Use(characterCard.Character);
                    foreach (var card in _squadMembersPanel.CharacterCards)
                    {
                        card.ResetSpecialClickEvent();
                    }
                });
            }
        }
    }
}