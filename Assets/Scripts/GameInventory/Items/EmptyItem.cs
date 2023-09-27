using System;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace GameInventory.Items
{
    public class EmptyItem : Item
    {
        public override ItemData Data => throw new Exception("Empty item has no data");

        public EmptyItem() : base(null)
        {
            
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            Debug.LogError("Empty item was taken");
        }
    }
}