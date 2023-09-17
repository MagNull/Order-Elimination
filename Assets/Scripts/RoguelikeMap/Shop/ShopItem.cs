using System;
using DG.Tweening;
using GameInventory.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RoguelikeMap.Shop
{
    public class ShopItem : MonoBehaviour
    {
        [SerializeField]
        private Image _image;
        [SerializeField] 
        private Button _button;
        [SerializeField]
        private float VisibilityAfterBuy = 0.5f;
        [SerializeField]
        private float Duration = 0.5f;

        private int _cost;
        private bool _isBuy;
        
        public ItemData Data { get; private set; }
        public int Cost => _cost;
        public bool IsBuy => _isBuy;
        public event Action<ShopItem> OnSelected;
        
        public void Initialize(ItemData data, int cost)
        {
            Data = data;
            _isBuy = false;
            _cost = cost;
            _image.sprite = data.View.Icon;
            GetComponentInChildren<TextMeshProUGUI>().text = _cost.ToString();
            _button.onClick.AddListener(() => OnSelected?.Invoke(this));
        }

        public void Buy()
        {
            _isBuy = true;
            _button.interactable = false;
            _image.DOFade(VisibilityAfterBuy, Duration);
        }
    }
}
