using UnityEngine;

namespace Ability.AbilityEffects
{
    public class DamageAbility : IAbility
    {
        private readonly IAbility _nextAbility;
        private readonly int _damage;

        public DamageAbility(IAbility nextAbility, int damage)
        {
            _nextAbility = nextAbility;
            _damage = damage;
        }

        public void Use(IBattleObject caster, IBattleObject target, BattleMapView battleMapView)
        {
            Debug.Log("Damage");
            //target.GetView().GetModel().TakeDamage(_damage);
            _nextAbility?.Use(caster, target, battleMapView);
        }
    }
}