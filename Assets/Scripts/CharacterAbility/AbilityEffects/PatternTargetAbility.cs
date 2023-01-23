using Cysharp.Threading.Tasks;
using OrderElimination;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class PatternTargetAbility : Ability
    {
        private readonly Vector2Int[] _pattern;
        private readonly BattleMap _battleMap;
        private readonly BattleObjectSide _filter;
        private readonly int _maxDistance;

        public PatternTargetAbility(IBattleObject caster, Vector2Int[] pattern, BattleMap battleMap, Ability nextEffect,
            BattleObjectSide filter, int maxDistance) :
            base(caster, false, nextEffect, filter)
        {
            _pattern = pattern;
            _battleMap = battleMap;
            _filter = filter;
            _maxDistance = maxDistance;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            var targetCoord =
                _battleMap.GetBattleObjectsInPatternArea(target, _caster, _pattern, _filter, _maxDistance);
            foreach (var patternTarget in targetCoord)
            {
                UseNext(patternTarget, stats);
            }
        }
    }
}