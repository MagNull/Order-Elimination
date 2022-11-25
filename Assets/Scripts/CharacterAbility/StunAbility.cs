using OrderElimination;

namespace CharacterAbility
{
    public class StunAbility : Ability
    {
        private readonly Ability _nextAbility;

        public StunAbility(IBattleObject caster, Ability nextAbility, float probability, BattleObjectSide filter) :
            base(caster, nextAbility, filter, probability)
        {
            _nextAbility = nextAbility;
        }

        
        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target is not IAbilityCaster abilityCaster)
                return;
            abilityCaster.ClearActions();
            _nextAbility?.Use(target, stats);
        }
    }
}