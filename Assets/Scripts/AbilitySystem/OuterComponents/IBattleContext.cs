using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleContext
    {
        #region Components
        public AnimationSceneContext AnimationSceneContext { get; }
        public IReadOnlyEntitiesBank EntitiesBank { get; }
        public EntitySpawner EntitySpawner { get; }
        public IBattleMap BattleMap { get; }
        #endregion

        #region Data
        public int CurrentRound { get; }
        public BattleSide ActiveSide { get; }
        #endregion

        #region Rules
        public ITurnPriority TurnPriority { get; }
        public IHitCalculation HitCalculation { get; }
        //public int MovementPointsPerRound { get; }
        //public int AttackPointsPerRound { get; }
        //public int ConsumablesPointsPerRound { get; }
        public int GetEnergyPointsPerRound(EnergyPoint pointType);
        public void SetEnergyPointsPerRound(EnergyPoint pointType, int valuePerRound);
        #endregion

        public BattleRelationship GetRelationship(BattleSide askingSide, BattleSide relationSide);
        public IEnumerable<AbilitySystemActor> GetVisibleEntities(BattleSide askingSide);
        public IEnumerable<AbilitySystemActor> GetVisibleEntitiesAt(Vector2Int position, BattleSide askingSide);
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