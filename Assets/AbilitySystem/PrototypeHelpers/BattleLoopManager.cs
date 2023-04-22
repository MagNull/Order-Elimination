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

public enum ActiveSide//TODO Replace with ActivePlayer
{
    Player,
    Allies,
    Enemies,
    Others
}

public class BattleLoopManager : SerializedMonoBehaviour
{
    private IObjectResolver _objectResolver;
    private BattleMapDirector _battleMapDirector;
    private IBattleMap _battleMap;

    public BattlePlayer ActivePlayer { get; private set; }
    public int CurrentRound { get; private set; }
    public event Action<int> NewRoundStarted;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _battleMap = objectResolver.Resolve<IBattleMap>();
        //_battleContext = objectResolver.Resolve<IBattleContext>();
    }

    public async void StartNewRound()
    {
        foreach (var pos in _battleMap.CellRangeBorders.EnumerateCellPositions())
        {
            foreach (var entity in _battleMap.GetContainingEntities(pos))
            {
                RestoreActionPoints(entity, 1);
                //entity.OnNewRoundCallback();
                //effects.OnNewRoundCallback();
                //*Или заменить подпиской на новый раунд внутри сущностей, эффектов и т.п.
            }
        }
        CurrentRound++;
        NewRoundStarted?.Invoke(CurrentRound);
        Debug.Log($"Round {CurrentRound} started.");
        //BattlePlayer currentPlayer = null;
        //await currentPlayer.OnTurnStarted(_battleContext);

        static void RestoreActionPoints(IAbilitySystemActor entity, int pointsToRestore)
        {
            foreach (var point in EnumExtensions.GetValues<ActionPoint>())
            {
                entity.SetActionPoints(point, pointsToRestore);
            }
        }
    }

    private void InitializeBattle()
    {
        var scenario = _objectResolver.Resolve<CharactersMediator>().BattleScenario;
        _objectResolver.Resolve<BattleInitializer>().InitiateBattle(scenario);
        NewRoundStarted += OnNewTurnTest;
        StartNewRound();
    }

    private void Start()
    {
        InitializeBattle();
    }

    private void OnNewTurnTest(int round)
    {
        var entities = _objectResolver.Resolve<BattleEntitiesBank>().GetEntities();
        var i = UnityEngine.Random.Range(0, entities.Length);
        var cells = _battleMap.CellRangeBorders.EnumerateCellPositions().ToArray();
        var cellId = UnityEngine.Random.Range(0, cells.Length);
        //entities[i].Move(cells[cellId]);
        entities[i].Move(cells[0]);
    }
}
