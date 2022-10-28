namespace Ability.AbilityEffects
{
    public class MoveAbility : IAbility
    {
        private readonly IAbility _nextAbility;

        public MoveAbility(IAbility nextAbility)
        {
            _nextAbility = nextAbility;
        }

        public void Use(IBattleObject caster, IBattleObject target, BattleMapView battleMapView)
        {
            //battleMapView.Map.MoveTo(caster, );
            _nextAbility?.Use(caster, target, battleMapView);
        }
    }
}