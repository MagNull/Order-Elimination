using Assets.AbilitySystem.PrototypeHelpers;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public class BattleLoopManager : MonoBehaviour
{
    private IObjectResolver _objectResolver;
    private IReadOnlyEntitiesBank _entitiesBank;
    //private IBattleContext _battleContext;

    //public BattlePlayer ActivePlayer { get; private set; }

    public BattleSide ActiveSide { get; private set; }
    public int CurrentRound { get; private set; }

    public event Action<BattleSide> NewTurnStarted;
    public event Action<int> NewRoundStarted;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        //_battleMap = objectResolver.Resolve<IBattleMap>();
        _entitiesBank = objectResolver.Resolve<IReadOnlyEntitiesBank>();
    }

    public async void StartNextTurn()
    {
        //ActiveSide = _battleContext.TurnPriority.GetNextTurnSide(ActiveSide);
        foreach (var entity in _entitiesBank.GetEntities().Where(e => e.BattleSide == ActiveSide))
        {
            RestoreActionPoints(entity, 1);
        }

        static void RestoreActionPoints(AbilitySystemActor entity, int pointsToRestore)
        {
            foreach (var point in EnumExtensions.GetValues<ActionPoint>())
            {
                entity.SetActionPoints(point, pointsToRestore);
            }
        }
    }

    public async void StartNewRound()
    {
        CurrentRound++;
        NewRoundStarted?.Invoke(CurrentRound);
        Debug.Log($"Round {CurrentRound} started.");
        //BattlePlayer currentPlayer = null;
        //await currentPlayer.OnTurnStarted(_battleContext);

        
    }

    private void InitializeBattle()
    {
        var scenario = _objectResolver.Resolve<CharactersMediator>().BattleScenario;
        //_battleContext = _objectResolver.Resolve<IBattleContext>();
        _objectResolver.Resolve<BattleInitializer>().InitiateBattle(scenario);
        //StartNewRound();
    }

    private void Start()
    {
        //InitializeBattle();
    }
}
