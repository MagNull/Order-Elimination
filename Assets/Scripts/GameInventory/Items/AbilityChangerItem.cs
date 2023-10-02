using System.Collections.Generic;
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
            var abilitiesIndeces = new Dictionary<ActiveAbilityRunner, int>();
            for (var i = 0; i < abilitySystemActor.ActiveAbilities.Count; i++)
            {
                abilitiesIndeces.Add(abilitySystemActor.ActiveAbilities[i], i);
            }
            foreach (var runner in abilitySystemActor.ActiveAbilities)
            {
                var builder = runner.AbilityData.BasedBuilder;
                if (_abilitySwapTable.ContainsKey(builder))
                {
                    abilitySystemActor.RemoveActiveAbility(runner);
                    abilitySystemActor.InsertActiveAbility(
                        abilitiesIndeces[runner],
                        new ActiveAbilityRunner(AbilityFactory.CreateActiveAbility(_abilitySwapTable[builder]), AbilityProvider.Self));
                }
            }
        }
    }
}