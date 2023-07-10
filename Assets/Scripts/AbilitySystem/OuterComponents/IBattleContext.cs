using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleContext
    {
        public AnimationSceneContext AnimationSceneContext { get; }
        public EntitySpawner EntitySpawner { get; }
        public IBattleMap BattleMap { get; }
        public IHitCalculation HitCalculation { get; }
        public ITurnPriority TurnPriority { get; }
        public BattleSide ActiveSide { get; }
        public int CurrentRound { get; }
        public IReadOnlyEntitiesBank EntitiesBank { get; }
        public BattleRelationship GetRelationship(BattleSide askingSide, BattleSide relationSide);
        public IEnumerable<AbilitySystemActor> GetVisibleEntities(Vector2Int position, BattleSide askingSide);
        public IContextValueGetter ModifyAccuracyBetween(
            Vector2Int start, Vector2Int end, IContextValueGetter initialAccuracy, AbilitySystemActor askingEntity);

        //public BattleRelationship GetRelationship(Player playerA, PlayerPrefs playerB)
        //public Player[] Players
        //public Player CurrentPlayer
        //public Player LocalPlayer (a player visuals locally drawn for - used for invisibility)
        //Characters
        //Structures

        public event Action<IBattleContext> BattleStarted;
        public event Action<IBattleContext> NewTurnUpdatesRequested;
        public event Action<IBattleContext> NewTurnStarted;
        public event Action<IBattleContext> NewRoundBegan;
        //public event Action<IBattleContext> ContextChanged;//?
    }
}