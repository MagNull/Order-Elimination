using System;
using System.Collections.Generic;
using Inventory;
using Inventory_Items;
using UnityEngine;
using VContainer;

namespace ItemsLibrary
{
    public class LibraryUI : MonoBehaviour
    {
        private IReadOnlyList<ItemView> _libraryItems;
        [SerializeField]private ItemType _typeOfLibrary;
        
        [SerializeField] private LibraryItemCell _item;
        [SerializeField] private int _slotsCount;

        [Inject]
        public void Configure(Library library)
        {
            _libraryItems = library.GetItems(_typeOfLibrary);
        }

        private void Start()
        {
            for (var i = 0; i < _slotsCount; i++)
            {
                var item = Instantiate(_item, transform);
                if (i < _libraryItems.Count)
                    item.SetViewSettings(_libraryItems[i]);
            }
        }
    }
}