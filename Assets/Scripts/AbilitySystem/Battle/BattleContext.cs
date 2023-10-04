﻿using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Battle;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

namespace OrderElimination.AbilitySystem
{
    public class BattleContext : IBattleContext
    {
        private BattleLoopManager _battleLoopManager;
        private Lazy<IBattleRules> _battleRules;
        private HashSet<UniTask> _executingTasks = new();

        #region Components
        public AnimationSceneContext AnimationSceneContext { get; private set; }
        public IReadOnlyEntitiesBank EntitiesBank { get; private set; }
        public EntitySpawner EntitySpawner { get; private set; }
        public IBattleMap BattleMap { get; private set; }
        #endregion

        #region Data
        public int CurrentRound => _battleLoopManager.CurrentRound;
        public BattleSide ActiveSide => _battleLoopManager.ActiveSide;
        public IBattleRules BattleRules => _battleRules.Value;
        #endregion

        public bool HasExecutingTasks => _executingTasks.Count > 0;
        public void AddExecutingTask(UniTask task)
        {
            if (_executingTasks.Contains(task))
                return;
            _executingTasks.Add(task);
            //task.ContinueWith(() => _executingTasks.Remove(task));//TODO: Timeout case
        }
        public void RemoveExecutingTask(UniTask task)
        {
            _executingTasks.Remove(task);
        }

        public event Action<IBattleContext> BattleStarted;
        public event Action<IBattleContext> NewTurnUpdatesRequested;
        public event Action<IBattleContext> NewTurnStarted;
        public event Action<IBattleContext> NewRoundBegan;

        [Inject]
        private void Construct(
            BattleLoopManager battleLoopManager,
            IBattleMap map,
            ScenesMediator mediator,
            IReadOnlyEntitiesBank entitiesBank,
            EntitySpawner entitySpawner,
            AnimationSceneContext animationContext)
        {
            BattleMap = map;
            EntitiesBank = entitiesBank;
            _battleLoopManager = battleLoopManager;
            _battleRules = new(() => 
            {
                return mediator.Get<IBattleRules>("rules"); 
            });
            AnimationSceneContext = animationContext;
            EntitySpawner = entitySpawner;
            _battleLoopManager.NewTurnStarted += OnNewTurn;
            _battleLoopManager.NewRoundBegan += OnNewRound;
            _battleLoopManager.BattleStarted += OnBattleStarted;

            async void OnNewTurn()
            {
                NewTurnUpdatesRequested?.Invoke(this);
                if (HasExecutingTasks)
                    await UniTask.WaitUntil(() => !HasExecutingTasks);
                NewTurnStarted?.Invoke(this);
            }
            void OnNewRound() => NewRoundBegan?.Invoke(this);
            void OnBattleStarted() => BattleStarted?.Invoke(this);
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

        public IEnumerable<AbilitySystemActor> GetVisibleEntities(BattleSide askingSide)
        {
            var visibleEntities = new List<AbilitySystemActor>();
            var entitiesSource = EntitiesBank.GetActiveEntities();
            foreach (var entity in entitiesSource)
            {
                var relationship = GetRelationship(askingSide, entity.BattleSide);
                if (!entity.StatusHolder.HasStatus(BattleStatus.Invisible)
                    || relationship == BattleRelationship.Ally)
                    visibleEntities.Add(entity);
            }
            return visibleEntities;
        }

        public IEnumerable<AbilitySystemActor> GetVisibleEntitiesAt(Vector2Int position, BattleSide askingSide)
        {
            var visibleEntities = new List<AbilitySystemActor>();
            var entitiesSource = BattleMap.GetContainedEntities(position);
            foreach (var entity in entitiesSource)
            {
                var relationship = GetRelationship(askingSide, entity.BattleSide);
                if (!entity.StatusHolder.HasStatus(BattleStatus.Invisible)
                    || relationship == BattleRelationship.Ally)
                    visibleEntities.Add(entity);
            }
            return visibleEntities;
        }

        public IContextValueGetter ModifyAccuracyBetween(
            Vector2Int start, Vector2Int end, IContextValueGetter initialAccuracy, AbilitySystemActor askingEntity)
        {
            var battleMap = BattleMap;
            if (!battleMap.ContainsPosition(start)
                || !battleMap.ContainsPosition(end))
                throw new ArgumentOutOfRangeException("Position is not presented in the BattleMap.");
            if (askingEntity.IsDisposedFromBattle)
                throw new InvalidOperationException("Attempt to calculate accuracy for disposed caster");
            var intersections = CellMath.GetIntersectionBetween(start, end);
            var modifiedAccuracy = initialAccuracy.Clone();
            foreach (var intersection in intersections)
            {
                var position = intersection.CellPosition;
                //cell 
                foreach (var battleObstacle in this
                    .GetVisibleEntitiesAt(position, askingEntity.BattleSide)//!!Considers affection outside obstacle class!!
                    .Select(e => e.Obstacle))
                {
                    //obstacle
                    var modification = battleObstacle.ModifyAccuracy(
                        modifiedAccuracy,
                        intersection.IntersectionAngle,
                        intersection.SmallestPartSquare,
                        askingEntity);
                    if (modification.IsModificationSuccessful)
                    {
                        modifiedAccuracy = modification.ModifiedValueGetter;
                    }
                }
            }
            return modifiedAccuracy;
        }
    }
}
