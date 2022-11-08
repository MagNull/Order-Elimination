using Cysharp.Threading.Tasks;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class MoveAbility : Ability
    {
        private readonly Ability _nextAbility;

        public MoveAbility(IAbilityCaster caster, Ability nextAbility) : base(caster)
        {
            _nextAbility = nextAbility;
        }

        public override void Use(IBattleObject target, IReadOnlyBattleStats stats, BattleMap battleMap)
        {
            var point = battleMap.GetCoordinate(target);
            battleMap.MoveTo(_caster, point.x, point.y);
            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}