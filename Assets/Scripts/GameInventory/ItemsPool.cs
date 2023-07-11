using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination
{
    [Serializable, CreateAssetMenu(fileName = "ItemsPool", menuName = "Inventory/ItemsPool")]
    public class ItemsPool : SerializedScriptableObject
    {
        [SerializeField]
        private ItemData[] _items;
        private static ItemData[] _staticItems;

        [SerializeField]
        private Dictionary<ItemRarity, float> _rarityProbability = new();
        private static Dictionary<ItemRarity, float> _staticRarityProbability;

        private void Awake()
        {
            _staticItems = _items;
            _staticRarityProbability = _rarityProbability;
        }

        private void OnEnable()
        {
            _staticItems = _items;
            _staticRarityProbability = _rarityProbability;
        }

        private void OnValidate()
        {
            _staticItems = _items;
            _staticRarityProbability = _rarityProbability;
        }

        public static Item GetRandomItem()
        {
            var randomRarity = GetRandomRarity();
            var items = _staticItems.Where(item => item.Rarity == randomRarity).ToList();

            var randomItemIndex = UnityEngine.Random.Range(0, items.Count);
            return ItemFactory.Create(items[randomItemIndex]);
        }
        
        private static ItemRarity GetRandomRarity()
        {
            var randomValue = UnityEngine.Random.value;
            var probabilitySum = 0f;
            foreach (var (rarity, probability) in _staticRarityProbability)
            {
                probabilitySum += probability;
                if (randomValue <= probabilitySum)
                    return rarity;
            }

            return ItemRarity.Common;
        }
    }
}