using System;
using AI;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace GameInventory.Items
{
    [Serializable]
    public class Item
    {
        [SerializeField, HideInInspector]
        private ItemData _itemData;

        public virtual ItemData Data => _itemData;

        public Item(ItemData itemData)
        {
            _itemData = itemData;
        }

        public virtual void OnTook(AbilitySystemActor abilitySystemActor)
        {
            return;
        }
    }

    public interface IUsable
    {
        public void Use(GameCharacter gameCharacter);

        public bool CheckConditionToUse(GameCharacter gameCharacter);
    }
}