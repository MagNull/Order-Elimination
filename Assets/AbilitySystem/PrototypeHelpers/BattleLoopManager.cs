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
    private BattleMapDirector _battleMapDirector;
    private CharacterArrangeDirector _characterArrangeDirector;
    private CharactersMediator _characterMediator;
    private IBattleMap _battleMap => _battleMapDirector.Map;
    private IBattleContext _battleContext;
    private IAbilitySystemActor[] _characters;

    public BattlePlayer ActivePlayer { get; private set; }
    public int CurrentTurn { get; private set; }
    public event Action<int> NewTurnStarted;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _battleMapDirector = objectResolver.Resolve<BattleMapDirector>();
        _characterArrangeDirector = objectResolver.Resolve<CharacterArrangeDirector>();
        _characterMediator = objectResolver.Resolve<CharactersMediator>();
        _battleContext = objectResolver.Resolve<IBattleContext>();
    }

    public void InitiateBattle(BattleScenario scenario)
    {
        var mapIndex = _battleMapDirector.InitializeMap();

        _characterArrangeDirector.SetArrangementMap(_battleMapDirector.Map);
        _characters = _characterArrangeDirector.ArrangeEntities(scenario.AlliesSpawnPositions, scenario.EnemySpawnPositions);
    }

    public async void StartNewTurn()
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
        CurrentTurn++;
        NewTurnStarted?.Invoke(CurrentTurn);

        static void RestoreActionPoints(IAbilitySystemActor entity, int pointsToRestore)
        {
            foreach (var point in EnumExtensions.GetValues<ActionPoint>())
            {
                entity.SetActionPoints(point, pointsToRestore);
            }
        }
        //BattlePlayer currentPlayer = null;
        //await currentPlayer.OnTurnStarted(_battleContext);
    }

    private void Start()
    {
        InitiateBattle();
        NewTurnStarted += OnNewTurnTest;
        StartNewTurn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartNewTurn();
        }
    }

    private void OnNewTurnTest(int round)
    {
        var i = UnityEngine.Random.Range(0, _characters.Length);
        var cells = _battleMap.CellRangeBorders.EnumerateCellPositions().ToArray();
        var cellId = UnityEngine.Random.Range(0, cells.Length);
        _characters[i].Move(cells[cellId]);
        _characters[i].Move(cells[0]);
    }
}
