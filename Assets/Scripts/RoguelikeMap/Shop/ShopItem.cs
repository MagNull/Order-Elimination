using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OrderElimination
{
    public class ShopItem : MonoBehaviour
    {
        private int _cost;
        private FakeAbilityBase _ability;
        public FakeAbilityBase Ability => _ability;
        public int Cost => _cost;
        public void Initialize(FakeAbilityBase ability)
        {
            _ability = ability;
            _cost = ability.Cost;
            GetComponent<Image>().sprite = ability.Sprite;
            GetComponentInChildren<TextMeshProUGUI>().text = _cost.ToString();
            GetComponent<Button>().onClick.AddListener(() => Shop.Buy(this));
        }
    }   
}
