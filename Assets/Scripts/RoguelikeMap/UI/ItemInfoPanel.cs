using System;
using GameInventory.Items;
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

        public event Action OnBuy;
        
        public void Initialize(ItemView _itemView)
        {
            _image.sprite = _itemView.Icon;
            _title.text = _itemView.Name;
            _description.text = _itemView.Description;
        }

        public void Buy()
        {
            OnBuy?.Invoke();
            Close();
        }
    }
}