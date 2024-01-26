using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace GameInventory.Views
{
    [Serializable]
    public class LastAddedItemsView : MonoBehaviour
    {
        [SerializeField]
        private List<Image> _cellsSprites;

        private Inventory _inventory;

        [Inject]
        public void Construct(Inventory inventory)
        {
            Debug.LogError("Injected");
            _inventory = inventory;
        }

        private void OnEnable()
        {
            if (_inventory != null)
            {
                _inventory.OnCellChanged += UpdateLastItems;
                UpdateLastItems(null);
            }
        }

        private void OnDisable()
        {
            if (_inventory != null)
            {
                _inventory.OnCellChanged -= UpdateLastItems;
            }
        }

        private void UpdateLastItems(IReadOnlyCell _)
        {
            var lastItems = _inventory.GetItems()
                .Where(x => x is not EmptyItem && !x.Data.HideInInventory)
                .TakeLast(_cellsSprites.Count)
                .ToArray();
            foreach (var cellsSprite in _cellsSprites)
                cellsSprite.enabled = false;
            for (var i = 0; i < lastItems.Length; i++)
            {
                _cellsSprites[i].enabled = true;
                _cellsSprites[i].sprite = lastItems[i].Data.View.Icon;
            }
        }
    }
}