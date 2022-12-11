using OrderElimination;

namespace CharacterAbility
{
    public class StunAbility : Ability
    {

        public StunAbility(IBattleObject caster, Ability effects, float probability, BattleObjectSide filter) :
            base(caster, effects, filter, probability)
        {
        }

        
        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target is not IActor abilityCaster)
                return;
            abilityCaster.ClearActions();
        }
    }
}