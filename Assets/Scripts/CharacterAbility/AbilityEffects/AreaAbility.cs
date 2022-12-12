using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class AreaAbility : Ability
    {
        private readonly int _radius;
        private readonly Ability _areaEffects;
        private readonly BattleMap _battleMap;

        public AreaAbility(IBattleObject caster, Ability areaEffects, BattleMap battleMap,
            int radius, BattleObjectSide filter) :
            base(caster, null, filter)
        {
            _radius = radius;
            _areaEffects = areaEffects;
            _battleMap = battleMap;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var targets = _battleMap.GetBattleObjectsInRadius(target, _radius);
            targets.Remove(target);
            foreach (var battleObject in targets)
            {
                _areaEffects.Use(battleObject, stats);
            }
        }
    }
}