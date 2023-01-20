using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.BM;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class MoveAbility : Ability
    {
        private readonly BattleMap _battleMap;
        private readonly float _stepDelay;

        public MoveAbility(IBattleObject caster, bool isMain, Ability nextEffect, float probability, BattleMap battleMap,
            BattleObjectSide filter, float stepDelay) : base(caster, isMain, nextEffect, filter, probability)
        {
            _battleMap = battleMap;
            _stepDelay = stepDelay;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            Debug.Log("Enter");
            Vector2Int targetPosition = _battleMap.GetCoordinate(target);
            if (target is not NullBattleObject && target is not EnvironmentObject)
            {
                var availablePositions = _battleMap.GetEmptyObjectsInRadius(target, 1);
                if (availablePositions.Count == 0)
                    return;
                var nearestPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                foreach (var availablePosition in availablePositions)
                {
                    if (_battleMap.GetStraightDistance(_caster, nearestPosition) >
                        _battleMap.GetStraightDistance(_caster, availablePosition))
                        nearestPosition = availablePosition;
                }

                targetPosition = _battleMap.GetCoordinate(nearestPosition);
            }

            var path = _battleMap.GetShortestPath(_caster, targetPosition.x, targetPosition.y);

            var turnEnded = false;
            CancellationTokenSource cancellationToken = new CancellationTokenSource();
            BattleSimulation.PlayerTurnEnd += () => cancellationToken.Cancel();
            var canceled = false;
            foreach (var cell in path)
            {
                canceled = await _battleMap.MoveTo(_caster, cell.x, cell.y, _stepDelay)
                    .AttachExternalCancellation(cancellationToken.Token).SuppressCancellationThrow();
            }

            if (canceled)
            {
                var last = path.Last();
                await _battleMap.MoveTo(_caster, last.x, last.y);
            }
            await UseNext(target, stats);
        }
    }
}