using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class HealAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _healAmount;
        private readonly float _attackScale;

        public HealAbility(IAbilityCaster caster, Ability nextAbility, int healAmount, float attackScale) : base(caster)
        {
            _attackScale = attackScale;
            _nextAbility = nextAbility;
            _healAmount = healAmount;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            BattleCharacter targetCharacter = target.GetView().GetComponent<BattleCharacterView>().Model;
            for (var i = 0; i > _healAmount; i++)
                targetCharacter.TakeHeal((int) (stats.Attack * _attackScale), stats.Accuracy);

            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}