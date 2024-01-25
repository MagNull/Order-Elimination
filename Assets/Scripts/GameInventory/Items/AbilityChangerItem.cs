using System;
using System.Collections.Generic;
using OrderElimination.AbilitySystem;

namespace GameInventory.Items
{
    public class AbilityChangerItem : Item
    {
        public AbilityChangerItem(ItemData itemData) : base(itemData)
        {
            if (itemData.AbilitySwapTable == null)
                throw new ArgumentException(
                    $"{nameof(itemData)} has no {nameof(itemData.AbilitySwapTable)} assigned.");
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            var swapTable = Data.AbilitySwapTable;
            var abilitiesIndeces = new Dictionary<ActiveAbilityRunner, int>();
            for (var i = 0; i < abilitySystemActor.ActiveAbilities.Count; i++)
            {
                abilitiesIndeces.Add(abilitySystemActor.ActiveAbilities[i], i);
            }
            foreach (var runner in abilitySystemActor.ActiveAbilities)
            {
                var builder = runner.AbilityData.BasedBuilder;
                if (swapTable.ContainsKey(builder))
                {
                    abilitySystemActor.RemoveActiveAbility(runner);
                    abilitySystemActor.InsertActiveAbility(
                        abilitiesIndeces[runner],
                        new ActiveAbilityRunner(
                            AbilityFactory.CreateActiveAbility(swapTable[builder]), 
                            AbilityProvider.Self));
                }
            }
        }
    }
}