using UnityEngine;

namespace Ability.AbilityEffects
{
    public class HealAbility : IAbility
    {
        private readonly IAbility _nextAbility;
        private readonly int _healAmount;

        public HealAbility(IAbility nextAbility, int healAmount)
        {
            _nextAbility = nextAbility;
            _healAmount = healAmount;
        }

        public void Use(IBattleObject caster, IBattleObject target, BattleMapView battleMapView)
        {
            Debug.Log("HealAbility");
            //target.GetView().GetModel().TakeHeal(_healAmount);
            _nextAbility?.Use(caster, target, battleMapView);
        }
    }
}