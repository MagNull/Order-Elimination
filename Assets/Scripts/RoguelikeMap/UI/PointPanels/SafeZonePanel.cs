using System;
using System.Collections.Generic;
using Inventory;
using TMPro;
using UnityEngine;
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
        
        public event Action<int> OnHealAccept;
        public event Action<IReadOnlyList<ItemData>> OnLootAccept;
        public event Action<bool> OnSafeZoneVisit;
        
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
            OnLootAccept?.Invoke(_items);
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