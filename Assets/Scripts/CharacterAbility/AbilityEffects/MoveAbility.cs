using OrderElimination;
using OrderElimination.BattleMap;
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
            Vector2Int targetPosition = battleMap.GetCoordinate(target);
            if (target is not NullBattleObject)
            {
                var availablePositions = battleMap.GetEmptyObjectsInRadius(target, 1);
                if (availablePositions.Count == 0)
                    return;
                var nearestPosition = availablePositions[Random.Range(0, availablePositions.Count)];
                foreach (var availablePosition in availablePositions)
                {
                    if (battleMap.GetDistance(_caster, nearestPosition) >
                        battleMap.GetDistance(_caster, availablePosition))
                        nearestPosition = availablePosition;
                }

                targetPosition = battleMap.GetCoordinate(nearestPosition);
            }

            battleMap.MoveTo(_caster, targetPosition.x, targetPosition.y);
            _nextAbility?.Use(target, stats, battleMap);
        }
    }
}