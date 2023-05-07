using Assets.AbilitySystem.PrototypeHelpers;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.AbilitySystem.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace OrderElimination.AbilitySystem
{
    public class BattleContext : IBattleContext
    {
        private BattleLoopManager _battleLoopManager;
        private IHitCalculation _hitCalculation = new StandartHitCalculation();

        public AnimationSceneContext AnimationSceneContext { get; private set; }
        public int CurrentRound => _battleLoopManager.CurrentRound;
        public IBattleMap BattleMap { get; private set; }
        public IHitCalculation HitCalculation => _hitCalculation;
        public IReadOnlyEntitiesBank EntitiesBank { get; private set; }

        public ITurnPriority TurnPriority { get; } = new PlayerFirstTurnPriority();

        public BattleSide ActiveSide => _battleLoopManager.ActiveSide;

        public event Action<IBattleContext> NewTurnStarted;
        public event Action<IBattleContext> NewRoundStarted;

        public BattleRelationship GetRelationship(AbilitySystemActor askingEntity, AbilitySystemActor relationEntity)
        {
            if (askingEntity.BattleSide == relationEntity.BattleSide)
                return BattleRelationship.Ally;
            return BattleRelationship.Enemy;
            throw new NotImplementedException();
        }

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            BattleMap = objectResolver.Resolve<IBattleMap>();
            EntitiesBank = objectResolver.Resolve<IReadOnlyEntitiesBank>();
            //_battleLoopManager = objectResolver.Resolve<BattleLoopManager>();
            AnimationSceneContext = objectResolver.Resolve<AnimationSceneContext>();
            //_battleLoopManager.NewRoundStarted += OnNewRound;
            void OnNewRound(int round)
            {
                NewRoundStarted?.Invoke(this);
            }
        }
    }
}
