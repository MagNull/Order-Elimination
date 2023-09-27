using System;
using GameInventory.Items;
using RoguelikeMap.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.UI
{
    public class ItemInfoPanel : Panel
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private TMP_Text _title;
        [SerializeField]
        private TMP_Text _description;
        [SerializeField]
        private Button _buyButton;
        [SerializeField]
        private Button _returnButton;

        public event Action OnBuy;
        public event Action OnReturn;
        
        public void Initialize(ShopItem shopItem)
        {
            _image.sprite = shopItem.Data.View.Icon;
            _title.text = shopItem.Data.View.Name;
            _description.text = shopItem.Data.View.Description;
            _buyButton.gameObject.SetActive(!shopItem.IsBuy);
            _returnButton.gameObject.SetActive(shopItem.IsBuy);
        }

        public void Buy()
        {
            OnBuy?.Invoke();
            Close();
        }

        public void Return()
        {
            OnReturn?.Invoke();
            Close();
        }
    }
}