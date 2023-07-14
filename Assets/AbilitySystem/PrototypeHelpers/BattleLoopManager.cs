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
using OrderElimination.MacroGame;
using UnityEngine;
using VContainer;

public class BattleLoopManager : MonoBehaviour
{
    private IObjectResolver _objectResolver;
    private IReadOnlyEntitiesBank _entitiesBank;
    private IBattleContext _battleContext;
    private BattleSide _activeSide;

    //public BattlePlayer ActivePlayer { get; private set; }
    public bool IsInitialized { get; private set; }

    public BattleSide ActiveSide
    {
        get
        {
            //if (!IsInitialized)
            //    throw new InvalidOperationException(
            //        $"Attempt to access {nameof(ActiveSide)} before {nameof(BattleLoopManager)} initialization.");
            return _activeSide;
        }
    }
    public int CurrentRound { get; private set; }

    public event Action BattleStarted;
    public event Action NewTurnStarted;
    public event Action NewRoundBegan;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _entitiesBank = objectResolver.Resolve<IReadOnlyEntitiesBank>();
    }

    private void Start()
    {
        InitializeBattle();
    }

    private void InitializeBattle()
    {
        IUndoableBattleAction.ClearAllActionsUndoCache();
        var scenario = _objectResolver.Resolve<ScenesMediator>().Get<BattleScenario>("scenario");
        _battleContext = _objectResolver.Resolve<IBattleContext>();
        var initializer = _objectResolver.Resolve<BattleInitializer>();
        initializer.InitiateBattle();
        initializer.StartScenario(scenario);
        _activeSide = _battleContext.TurnPriority.GetStartingSide();
        IsInitialized = true;
        StartNewTurn(ActiveSide);
        BattleStarted?.Invoke();
    }

    public void StartNextTurn()
    {
        StartNewTurn(_battleContext.TurnPriority.GetNextTurnSide(ActiveSide));
    }

    private void StartNewTurn(BattleSide battleSide)
    {
        _activeSide = battleSide;
        foreach (var entity in _entitiesBank.GetActiveEntities(ActiveSide))
        {
            RestoreActionPoints(entity, 1);
        }

        if (ActiveSide == _battleContext.TurnPriority.GetStartingSide())
        {
            CurrentRound++;
            NewRoundBegan?.Invoke();
        }
        NewTurnStarted?.Invoke();
        //if (_entitiesBank.GetEntities().Length > 0 
        //    && !_entitiesBank.GetEntities().Any(e => e.BattleSide == ActiveSide))
        //    StartNextTurn();

        static void RestoreActionPoints(AbilitySystemActor entity, int pointsToRestore)
        {
            foreach (var point in EnumExtensions.GetValues<EnergyPoint>())
            {
                entity.SetEnergyPoints(point, pointsToRestore);
            }
        }
    }
}
