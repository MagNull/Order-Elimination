using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class HealAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _healAmount;

        public HealAbility(BattleCharacter caster, Ability nextAbility, int healAmount) : base(caster)
        {
            _nextAbility = nextAbility;
            _healAmount = healAmount;
        }

        public override void Use(IBattleObject target, BattleMap battleMap)
        {
            Debug.Log("HealAbility");
            //target.GetView().GetModel().TakeHeal(_healAmount);
            _nextAbility?.Use(target, battleMap);
        }
    }
}