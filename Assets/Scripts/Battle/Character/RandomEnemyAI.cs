using System;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BM;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIAbility
{
    public readonly int Distance;
    public readonly TargetType TargetType;
    private readonly Ability _ability;
    public readonly string AbilityName;
    private int _coolDown;
    private int _currentCoolDownTick;

    public bool CanCast => _currentCoolDownTick <= 0;

    public AIAbility(int distance, TargetType targetType, Ability ability, string abilityName, int coolDown,
        int startCoolDown)
    {
        Distance = distance;
        TargetType = targetType;
        _ability = ability;
        AbilityName = abilityName;
        _coolDown = coolDown;
        _currentCoolDownTick = startCoolDown;
    }

    public void Use(IBattleObject target, IReadOnlyBattleStats stats)
    {
        if (!CanCast)
            return;

        _ability.Use(target, stats);
        _currentCoolDownTick = _coolDown;
    }

    public void Tick() => _currentCoolDownTick--;
}

public class RandomEnemyAI : BattleCharacter
{
    private readonly BattleMap _map;
    private readonly IReadOnlyCharacterBank _characterBank;
    private Ability _moveAbility;
    private List<AIAbility> _abilities;
    private AIAbility _currentAIAbility;

    public RandomEnemyAI(BattleMap map, OldBattleStats battleStats, IDamageCalculation damageCalculation,
        IReadOnlyCharacterBank characterBank)
        : base(BattleObjectType.Enemy, battleStats, damageCalculation)
    {
        _map = map;
        _characterBank = characterBank;
    }

    public void SetAbilities(List<AIAbility> abilityAIInfo)
    {
        _abilities = abilityAIInfo;
    }

    public void SetMoveAbility(Ability moveAbility)
    {
        _moveAbility = moveAbility;
    }

    public override async UniTask PlayTurn()
    {
        TickCoolDowns();
        var nearestPlayer = SearchNearestPlayer(_characterBank.GetAllies());
        ChooseAbilityOnRound();
        if (_currentAIAbility == null)
            return;
        if (TryAttack(nearestPlayer)) return;
        await Move(GetOptimalCoordinateToMove(nearestPlayer));
        TryAttack(nearestPlayer);
    }

    private void TickCoolDowns()
    {
        foreach (var abilityAI in _abilities)
        {
            abilityAI.Tick();
        }
    }

    private void ChooseAbilityOnRound()
    {
        _currentAIAbility = null;
        var availableAbility = _abilities.Where(x => x.CanCast).ToList();
        if (availableAbility.Count == 0)
            return;
        _currentAIAbility = availableAbility[Random.Range(0, availableAbility.Count)];
        Logging.Log(_currentAIAbility.AbilityName);
    }

    private bool TryAttack(BattleCharacter nearestPlayer)
    {
        if (!CanSpendAction(ActionType.Ability))
            return false;
        switch (_currentAIAbility.TargetType)
        {
            case TargetType.Self:
                _currentAIAbility.Use(this, Stats);
                break;
            case TargetType.Ally:
                var allies =
                    _map.GetBattleObjectsInRadius(nearestPlayer, _currentAIAbility.Distance, BattleObjectType.Enemy);
                if (allies.Count == 0 || _map.GetStraightDistance(this, allies[0]) > _currentAIAbility.Distance)
                    _currentAIAbility.Use(this, Stats);
                else
                    _currentAIAbility.Use(allies[0], Stats);

                break;
            case TargetType.All:
            case TargetType.Enemy:
                if (_map.GetStraightDistance(this, nearestPlayer) > _currentAIAbility.Distance)
                    return false;
                _currentAIAbility.Use(nearestPlayer, Stats);
                break;
        }

        return true;
    }

    private BattleCharacter SearchNearestPlayer(IEnumerable<BattleCharacter> players)
    {
        BattleCharacter nearestEnemy = null;
        var minDistance = int.MaxValue;
        foreach (var player in players)
        {
            var distance = _map.GetStraightDistance(this, player);
            if (minDistance <= distance)
                continue;
            nearestEnemy = player;
            minDistance = distance;
        }

        Logging.Log(nearestEnemy.View.GameObject.name);

        return nearestEnemy;
    }

    private Vector2Int GetOptimalCoordinateToMove(BattleCharacter player)
    {
        var distance = _map.GetStraightDistance(this, player);
        var optimalDistance = _currentAIAbility.Distance;
        if (distance <= optimalDistance)
            return _map.GetCoordinate(this);
        var optimalPosition = _map.GetOptimalPosition(this, player, (int)Stats.Movement, optimalDistance);
        return optimalPosition;
    }

    private async UniTask Move(Vector2Int coordinate)
    {
        if (!TrySpendAction(ActionType.Movement))
            return;
        await _moveAbility.Use(_map.GetCell(coordinate.x, coordinate.y).Objects[0], Stats);
    }
}