using System;
using System.Collections.Generic;
using GameInventory;
using GameInventory.Items;
using TMPro;
using UnityEngine;
using VContainer;
using Image = UnityEngine.UI.Image;

namespace RoguelikeMap.UI.PointPanels
{
    public class SafeZonePanel : Panel
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _text;

        private int _amountHeal;
        private IReadOnlyList<ItemData> _items;
        private Inventory _inventory;
        
        public event Action<int> OnHealAccept;
        public event Action<bool> OnSafeZoneVisit;
        
        [Inject]
        public void Construct(Inventory inventory)
        {
            _inventory = inventory;
        }

        public void SetInfo(int amountHeal, IReadOnlyList<ItemData> items, Sprite sprite, string text)
        {
            _amountHeal = amountHeal;
            _items = items;
            _image.sprite = sprite;
            _text.text = text;
        }

        public void HealAccept()
        {
            OnHealAccept?.Invoke(_amountHeal);
            Close();
        }

        //TODO(coder): add loot to player inventory after create inventory system
        public void LootAccept()
        {
            foreach (var itemData in _items)
            {
                var item = ItemFactory.Create(itemData);
                _inventory.AddItem(item);
            }
            Close();
        }

        public override void Open()
        {
            base.Open();
            OnSafeZoneVisit?.Invoke(true);
        }
        
        public override void Close()
        {
            OnSafeZoneVisit?.Invoke(false);
            base.Close();
        }
    }
}