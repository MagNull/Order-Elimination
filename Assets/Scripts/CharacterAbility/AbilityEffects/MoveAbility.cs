﻿using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.BattleMap;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class MoveAbility : Ability
    {
        private readonly Ability _nextEffect;
        private readonly BattleMap _battleMap;
        private readonly float _stepDelay;

        public MoveAbility(IBattleObject caster, Ability nextEffect, float probability, BattleMap battleMap,
            BattleObjectSide filter, float stepDelay) : base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _battleMap = battleMap;
            _stepDelay = stepDelay;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            Vector2Int targetPosition = _battleMap.GetCoordinate(target);
            if (target is not NullBattleObject && target is not EnvironmentObject)
            {
                var availablePositions = _battleMap.GetEmptyObjectsInRadius(target, 1);
                if (availablePositions.Count == 0)
                    return;
                var nearestPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                foreach (var availablePosition in availablePositions)
                {
                    if (_battleMap.GetDistance(_caster, nearestPosition) >
                        _battleMap.GetDistance(_caster, availablePosition))
                        nearestPosition = availablePosition;
                }

                targetPosition = _battleMap.GetCoordinate(nearestPosition);
            }

            var path = _battleMap.GetShortestPath(_caster, targetPosition.x, targetPosition.y);
            foreach (var cell in path)
            {
                await _battleMap.MoveTo(_caster, cell.x, cell.y, _stepDelay);
            }
            
            if(_nextEffect == null)
                return;
            await _nextEffect.Use(target, stats);
            
        }
    }
}