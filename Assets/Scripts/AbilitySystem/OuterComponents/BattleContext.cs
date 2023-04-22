using Assets.AbilitySystem.PrototypeHelpers;
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

        public IObjectResolver ObjectResolver { get; private set; }
        public int CurrentRound => _battleLoopManager.CurrentRound;
        public IBattleMap BattleMap { get; private set; }
        public IHitCalculation HitCalculation => _hitCalculation;
        public BattleEntitiesBank EntitiesBank { get; private set; }

        public event Action<IBattleContext> NewRoundStarted;

        public BattleSide GetRelationship(IAbilitySystemActor entityA, IAbilitySystemActor entityB)
        {
            throw new NotImplementedException();
        }

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            ObjectResolver = objectResolver;
            BattleMap = objectResolver.Resolve<IBattleMap>();
            EntitiesBank = objectResolver.Resolve<BattleEntitiesBank>();
            _battleLoopManager = objectResolver.Resolve<BattleLoopManager>();
            _battleLoopManager.NewRoundStarted += OnNewRound;
            void OnNewRound(int round)
            {
                NewRoundStarted?.Invoke(this);
            }
        }
    }
}
