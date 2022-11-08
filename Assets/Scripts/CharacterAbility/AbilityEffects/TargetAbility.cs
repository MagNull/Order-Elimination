using Cysharp.Threading.Tasks;
using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class TargetAbility : Ability
    {
        private readonly Ability _nextAbility;
        private readonly int _distance;
        private readonly bool _selfCast;

        public TargetAbility(IAbilityCaster caster, Ability nextAbility, int distance, bool selfCast) 
            : base(caster)
        {
            _nextAbility = nextAbility;
            _distance = distance;
            _selfCast = selfCast;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            _nextAbility.Use(_selfCast ? _caster : target, stats, battleMap);
        }
    }
}