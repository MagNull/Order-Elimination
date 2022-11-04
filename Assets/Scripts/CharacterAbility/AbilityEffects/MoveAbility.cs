using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class MoveAbility : Ability
    {
        private readonly Ability _nextAbility;

        public MoveAbility(BattleCharacter caster, Ability nextAbility) : base(caster)
        {
            _nextAbility = nextAbility;
        }

        public override void Use(IBattleObject target, BattleMap battleMap)
        {
            var point = battleMap.GetCoordinate(target);
            battleMap.MoveTo(_caster, point.x, point.y);
            _nextAbility?.Use(target, battleMap);
        }
    }
}