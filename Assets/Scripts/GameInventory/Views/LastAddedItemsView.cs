using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GameInventory.Views
{
    public class LastAddedItemsView : MonoBehaviour
    {
        [SerializeField]
        private List<Image> _cellsSprites;

        private Inventory _inventory;

        [Inject]
        public void Construct(Inventory inventory)
        {
            _inventory = inventory;
        }

        private void OnEnable()
        {
            _inventory.OnCellAdded += UpdateLastItems;
            _inventory.OnCellRemoved += UpdateLastItems;
            UpdateLastItems(null);
        }

        private void OnDisable()
        {
            _inventory.OnCellAdded -= UpdateLastItems;
            _inventory.OnCellRemoved -= UpdateLastItems;
        }

        private void UpdateLastItems(IReadOnlyCell cell)
        {
            var lastItems = _inventory.GetItems().TakeLast(_cellsSprites.Count).ToArray();
            for (var i = 0; i < lastItems.Length; i++)
            {
                _cellsSprites[i].sprite = lastItems[i].View.Icon;
            }

            foreach (var cellsSprite in _cellsSprites) 
                cellsSprite.enabled = cellsSprite.sprite != null;
        }
    }
}