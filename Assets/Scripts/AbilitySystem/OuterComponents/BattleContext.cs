using Assets.AbilitySystem.PrototypeHelpers;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
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
        public EntitySpawner EntitySpawner { get; private set; }
        public int CurrentRound => _battleLoopManager.CurrentRound;
        public IBattleMap BattleMap { get; private set; }
        public IHitCalculation HitCalculation => _hitCalculation;
        public IReadOnlyEntitiesBank EntitiesBank { get; private set; }

        public ITurnPriority TurnPriority { get; } = new PlayerFirstTurnPriority();

        public BattleSide ActiveSide => _battleLoopManager.ActiveSide;

        public event Action<IBattleContext> NewTurnStarted;
        public event Action<IBattleContext> NewRoundBegan;

        //TODO: Refactor
        public static IBattleContext CurrentSceneContext { get; private set; }
        //

        [Inject]
        private void Construct(IObjectResolver objectResolver)
        {
            BattleMap = objectResolver.Resolve<IBattleMap>();
            EntitiesBank = objectResolver.Resolve<IReadOnlyEntitiesBank>();
            _battleLoopManager = objectResolver.Resolve<BattleLoopManager>();
            AnimationSceneContext = objectResolver.Resolve<AnimationSceneContext>();
            EntitySpawner = objectResolver.Resolve<EntitySpawner>();
            _battleLoopManager.NewTurnStarted += OnNewTurn;
            _battleLoopManager.NewRoundBegan += OnNewRound;

            //TODO: Refactor
            CurrentSceneContext = this;
            //TODO: Refactor

            void OnNewTurn()
            {
                NewTurnStarted?.Invoke(this);
            }
            void OnNewRound()
            {
                NewRoundBegan?.Invoke(this);
            }
        }

        public BattleRelationship GetRelationship(BattleSide askingSide, BattleSide relationSide)
        {
            if (askingSide == BattleSide.NoSide || relationSide == BattleSide.NoSide)
                return BattleRelationship.Neutral;
            
            var playerFriends = new HashSet<BattleSide>() { BattleSide.Player, BattleSide.Allies };
            var enemiesFriends = new HashSet<BattleSide>() { BattleSide.Enemies };
            var othersFriends = new HashSet<BattleSide>() { BattleSide.Others };
            var noSideFriends = new HashSet<BattleSide>() { };
            var friendly = askingSide switch
            {
                BattleSide.NoSide => noSideFriends.Contains(relationSide),
                BattleSide.Player => playerFriends.Contains(relationSide),
                BattleSide.Enemies => enemiesFriends.Contains(relationSide),
                BattleSide.Allies => playerFriends.Contains(relationSide),
                BattleSide.Others => othersFriends.Contains(relationSide),
                _ => throw new NotImplementedException(),
            };
            if (friendly)
                return BattleRelationship.Ally;
            return BattleRelationship.Enemy;
        }

        public IEnumerable<AbilitySystemActor> GetVisibleEntities(Vector2Int position, BattleSide askingSide)
        {
            var visibleEntities = new List<AbilitySystemActor>();
            foreach (var entity in BattleMap.GetContainedEntities(position))
            {
                var relationship = GetRelationship(askingSide, entity.BattleSide);
                if (!entity.StatusHolder.HasStatus(BattleStatus.Invisible)
                    || relationship == BattleRelationship.Ally)
                    visibleEntities.Add(entity);
            }
            return visibleEntities;
        }
    }
}
