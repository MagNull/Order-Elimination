using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace GameInventory.Items
{
    public class AbilityChangerItem : Item
    {
        [SerializeField]
        private Dictionary<ActiveAbilityBuilder, ActiveAbilityBuilder> _abilitySwapTable = new();

        public AbilityChangerItem(ItemData itemData) : base(itemData)
        {
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            foreach (var swapPair in _abilitySwapTable)
            {
                var ability =
                    abilitySystemActor.ActiveAbilities.First(ab => ab.AbilityData.BasedBuilder == swapPair.Key);
                abilitySystemActor.RemoveActiveAbility(ability);
                abilitySystemActor.GrantActiveAbility(
                    new ActiveAbilityRunner(AbilityFactory.CreateActiveAbility(swapPair.Value), AbilityProvider.Self));
            }
        }
    }
}