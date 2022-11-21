﻿using OrderElimination;

namespace CharacterAbility.AbilityEffects
{
    public class AreaAbility : Ability
    {
        private readonly int _radius;
        private readonly Ability _areaAbility;
        private readonly BattleMap _battleMap;

        public AreaAbility(IBattleObject caster, Ability areaAbility, BattleMap battleMap,
            int radius, BattleObjectSide filter) :
            base(caster, null, filter, 100)
        {
            _radius = radius;
            _areaAbility = areaAbility;
            _battleMap = battleMap;
        }

        protected override void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var targets = _battleMap.GetBattleObjectsInRadius(target, _radius);
            targets.Remove(target);
            foreach (var battleObject in targets)
            {
                _areaAbility.Use(battleObject, stats);
            }
        }
    }
}