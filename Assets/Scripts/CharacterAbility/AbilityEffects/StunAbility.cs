using OrderElimination;

namespace CharacterAbility
{
    public class StunAbility : Ability
    {
        private readonly Ability _nextEffect;

        public StunAbility(IBattleObject caster, Ability nextEffect, float probability, BattleObjectSide filter) :
            base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
        }

        
        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target is not IActor abilityCaster)
            {
                _nextEffect?.Use(target, stats);
                return;
            }
            abilityCaster.ClearActions();
        }
    }
}