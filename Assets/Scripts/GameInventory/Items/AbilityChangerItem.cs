using System.Collections.Generic;
using System.Linq;
using OrderElimination;
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
            _abilitySwapTable = itemData.AbilitySwapTable;
        }

        public override void OnTook(AbilitySystemActor abilitySystemActor)
        {
            foreach (var swapPair in _abilitySwapTable)
            {
                var ability =
                    abilitySystemActor.ActiveAbilities.FirstOrDefault(ab =>
                        ab.AbilityData.BasedBuilder == swapPair.Key);
                if (ability == null)
                {
                    Logging.LogWarning("Try swap ability "+ ability.AbilityData.View.Name + ". There's no such ability");
                    return;
                }
                //TODO(Денис): Implement ability swap
                abilitySystemActor.RemoveActiveAbility(ability);
                abilitySystemActor.GrantActiveAbility(
                    new ActiveAbilityRunner(AbilityFactory.CreateActiveAbility(swapPair.Value), AbilityProvider.Self));
            }
        }
    }
}