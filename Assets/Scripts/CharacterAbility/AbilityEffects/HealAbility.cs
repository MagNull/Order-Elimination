using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class HealAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _healAmount;
        private readonly float _attackScale;

        public HealAbility(BattleCharacter caster, Ability nextAbility, int healAmount, float attackScale) : base(caster)
        {
            _attackScale = attackScale;
            _nextAbility = nextAbility;
            _healAmount = healAmount;
        }

        public override void Use(IBattleObject target, BattleMap battleMap)
        {
            BattleCharacter targetCharacter = target.GetView().GetComponent<BattleCharacterView>().Model;
            for(var i = 0; i > _healAmount; i++)
                targetCharacter.TakeHeal((int) (_caster.Stats.Attack * _attackScale), _caster.Stats.Accuracy);
            
            _nextAbility?.Use(target, battleMap);
        }
    }
}