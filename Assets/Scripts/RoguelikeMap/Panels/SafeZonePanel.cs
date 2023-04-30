using System;
using System.Collections.Generic;
using Inventory_Items;
using RoguelikeMap.Points;
using RoguelikeMap.Points.Models;
using TMPro;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace RoguelikeMap.Panels
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

        public override void SetInfo(PointModel model)
        {
            if (model is not SafeZonePointModel safeZoneModel)
                throw new ArgumentException("Is not valid PointInfo");
            SetInfo(safeZoneModel);
        }

        private void SetInfo(SafeZonePointModel safeZoneModel)
        {
            _amountHeal = safeZoneModel.AmountHeal;
            _items = safeZoneModel.Items;
            _image.sprite = safeZoneModel.Sprite;
            _text.text = safeZoneModel.Text;
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
    }
}