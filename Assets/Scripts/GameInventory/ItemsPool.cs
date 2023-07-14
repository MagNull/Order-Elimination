using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory;
using GameInventory.Items;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination
{
    [Serializable, CreateAssetMenu(fileName = "ItemsPool", menuName = "Inventory/ItemsPool")]
    public class ItemsPool : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<ItemRarity, float> _rarityProbability = new();

        public Item GetRandomItem()
        {
            var randomRarity = GetRandomRarity();
            var items = ItemIdentifier.GetItems().Where(item => item.Rarity == randomRarity).ToList();

            var randomItemIndex = UnityEngine.Random.Range(0, items.Count);
            return ItemFactory.Create(items[randomItemIndex]);
        }
        
        private ItemRarity GetRandomRarity()
        {
            var randomValue = UnityEngine.Random.value;
            var probabilitySum = 0f;
            foreach (var (rarity, probability) in _rarityProbability)
            {
                probabilitySum += probability / 100;
                if (randomValue <= probabilitySum)
                    return rarity;
            }

            return ItemRarity.Common;
        }
    }
}