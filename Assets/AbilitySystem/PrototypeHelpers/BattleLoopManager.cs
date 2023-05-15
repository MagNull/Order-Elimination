using Assets.AbilitySystem.PrototypeHelpers;
using Cysharp.Threading.Tasks;
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
    private IBattleContext _battleContext;

    //public BattlePlayer ActivePlayer { get; private set; }

    public BattleSide ActiveSide { get; private set; }
    public int CurrentRound { get; private set; }

    public event Action NewTurnStarted;
    public event Action NewRoundBegan;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        //_battleMap = objectResolver.Resolve<IBattleMap>();
        _entitiesBank = objectResolver.Resolve<IReadOnlyEntitiesBank>();
    }

    public void StartNextTurn()
    {
        StartNewTurn(_battleContext.TurnPriority.GetNextTurnSide(ActiveSide));
    }

    private void InitializeBattle()
    {
        var scenario = _objectResolver.Resolve<CharactersMediator>().BattleScenario;
        _battleContext = _objectResolver.Resolve<IBattleContext>();
        _objectResolver.Resolve<BattleInitializer>().InitiateBattle(scenario);
        IUndoableBattleAction.ClearAllActionsUndoCache();
        StartNewTurn(_battleContext.TurnPriority.GetStartingSide());
    }

    private void StartNewTurn(BattleSide battleSide)
    {
        ActiveSide = battleSide;
        foreach (var entity in _entitiesBank.GetEntities().Where(e => e.BattleSide == ActiveSide))
        {
            RestoreActionPoints(entity, 1);
        }

        if (ActiveSide == _battleContext.TurnPriority.GetStartingSide())
        {
            CurrentRound++;
            NewRoundBegan?.Invoke();
            Debug.Log($"Round {CurrentRound} began.");
        }
        NewTurnStarted?.Invoke();
        Debug.Log($"Turn of {ActiveSide} started.");
        if (!_entitiesBank.GetEntities().Any(e => e.BattleSide == ActiveSide))
            StartNextTurn();

        static void RestoreActionPoints(AbilitySystemActor entity, int pointsToRestore)
        {
            foreach (var point in EnumExtensions.GetValues<ActionPoint>())
            {
                entity.SetActionPoints(point, pointsToRestore);
            }
        }
    }

    private void Start()
    {
        InitializeBattle();
    }
}