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

        public MoveAbility(IBattleObject caster, Ability nextEffect, float probability, BattleMap battleMap,
            BattleObjectSide filter) : base(caster, nextEffect, filter, probability)
        {
            _nextEffect = nextEffect;
            _battleMap = battleMap;
        }

        protected override async void ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
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
                await _battleMap.MoveTo(_caster, cell.x, cell.y);
            }
            _nextEffect?.Use(target, stats);
        }
    }
}