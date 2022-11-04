using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class DamageAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _damage;

        public DamageAbility(BattleCharacter caster, Ability nextAbility, int damage) : base(caster)
        {
            _nextAbility = nextAbility;
            _damage = damage;
        }

        public override void Use(IBattleObject target, BattleMap battleMap)
        {
            Debug.Log("Damage");
            target.GetView().GetComponent<BattleCharacterView>().Model.TakeDamage(_damage);
            _nextAbility?.Use(target, battleMap);
        }
    }
}