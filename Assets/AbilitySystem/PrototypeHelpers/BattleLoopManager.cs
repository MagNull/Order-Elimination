using Assets.AbilitySystem.PrototypeHelpers;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Linq;
using OrderElimination.MacroGame;
using UnityEngine;
using VContainer;
using OrderElimination.Battle;

public class BattleLoopManager : MonoBehaviour
{
    private IObjectResolver _objectResolver;
    private IReadOnlyEntitiesBank _entitiesBank;
    private IBattleContext _battleContext;
    private BattleSide _activeSide;
    private EnergyPoint[] _energyPointTypes = EnumExtensions.GetValues<EnergyPoint>().ToArray();

    [SerializeField]
    private Vector2Int MapSize = new Vector2Int(8, 8);

    //public BattlePlayer ActivePlayer { get; private set; }
    public BattleSide ActiveSide => _activeSide;
    public int CurrentRound { get; private set; }
    private IBattleRules Rules => _battleContext.BattleRules;

    [Obsolete("Only for BattleContext use!")]
    public event Action BattleStarted;
    [Obsolete("Only for BattleContext use!")]
    public event Action NewTurnStarted;
    [Obsolete("Only for BattleContext use!")]
    public event Action NewRoundBegan;

    [Inject]
    private void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _entitiesBank = objectResolver.Resolve<IReadOnlyEntitiesBank>();
    }

    private void Start()
    {
        InitializeBattle(
            _objectResolver.Resolve<ScenesMediator>(),
            _objectResolver.Resolve<IBattleContext>(),
            _objectResolver.Resolve<BattleInitializer>());
    }

    private void InitializeBattle(
        ScenesMediator scenesMediator, IBattleContext battleContext, BattleInitializer battleInitializer)
    {
        IUndoableBattleAction.ClearAllActionsUndoCache();
        var scenario = scenesMediator.Get<BattleScenario>("scenario");
        _battleContext = battleContext;
        battleInitializer.InitiateBattle(MapSize.x, MapSize.y);
        //battleInitializer.InitiateBattle(scenario.MapWidth, scenario.MapWidth);
        _activeSide = Rules.TurnPriority.GetStartingSide();
        battleInitializer.StartScenario(scenario);
        //_entitiesBank.GetActiveEntities()
        //    .ForEach(e => e.PassiveAbilities.ForEach(a => a.Activate(_battleContext, e)));
        StartNewTurn(ActiveSide);
        //ActivatePassiveAbilities
        BattleStarted?.Invoke();
    }

    public void StartNextTurn()
    {
        StartNewTurn(Rules.TurnPriority.GetNextTurnSide(ActiveSide));
    }

    private void StartNewTurn(BattleSide battleSide)
    {
        _activeSide = battleSide;
        foreach (var entity in _entitiesBank.GetActiveEntities(ActiveSide))
        {
            RestoreEnergyPoints(entity);
        }

        if (ActiveSide == Rules.TurnPriority.GetStartingSide())
        {
            CurrentRound++;
            NewRoundBegan?.Invoke();
        }
        NewTurnStarted?.Invoke();
        //if (_entitiesBank.GetEntities().Length > 0 
        //    && !_entitiesBank.GetEntities().Any(e => e.BattleSide == ActiveSide))
        //    StartNextTurn();

        void RestoreEnergyPoints(AbilitySystemActor entity)
        {
            foreach (var point in _energyPointTypes)
            {
                var currentPoints = entity.EnergyPoints[point];
                var pointsToRestore = Rules.GetEnergyPointsPerRound(point);
                if (currentPoints < pointsToRestore || Rules.HardResetEnergyPointsEveryRound)
                    entity.SetEnergyPoints(point, pointsToRestore);
            }
        }
    }
}
