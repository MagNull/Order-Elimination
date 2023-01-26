using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.BM;
using UnityEngine;

namespace CharacterAbility.AbilityEffects
{
    public class ObjectSpawnAbility : Ability
    {
        private readonly EnvironmentInfo _environmentInfo;
        private readonly EnvironmentFactory _environmentFactor;
        private readonly EnvironmentFactory _environmentFactory;
        private readonly int _lifeTime;
        private readonly BattleMap _battleMap;

        public ObjectSpawnAbility(IBattleObject caster, EnvironmentInfo environmentInfo,
            EnvironmentFactory environmentFactory, int lifeTime, BattleMap battleMap,
            bool isMain, Ability nextEffect,
            BattleObjectSide filter, float probability = 100) : base(caster, isMain, nextEffect, filter, probability)
        {
            _environmentInfo = environmentInfo;
            _environmentFactory = environmentFactory;
            _lifeTime = lifeTime;
            _battleMap = battleMap;
        }

        protected override async UniTask ApplyEffect(IBattleObject target, IReadOnlyBattleStats stats)
        {
            if (target is EnvironmentObject)
            {
                await UseNext(target, stats);
                return;
            }

            var targetCoordinate = _battleMap.GetCoordinate(target);
            var envObject = _environmentFactory.Create(_environmentInfo, _lifeTime);
            await _battleMap.MoveTo(envObject, targetCoordinate.x, targetCoordinate.y, 0);
            if (target is not NullBattleObject or EnvironmentObject)
                await _battleMap.MoveTo(target, targetCoordinate.x, targetCoordinate.y, 0);
            await UseNext(_battleMap.GetCell(targetCoordinate.x, targetCoordinate.y).GetObject(), stats);
        }
    }
}