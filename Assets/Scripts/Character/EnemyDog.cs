using System;
using System.Collections.Generic;
using System.Linq;
using CharacterAbility;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.Battle;
using OrderElimination.BattleMap;
using UnityEngine;

public class EnemyDog : BattleCharacter
{
    private BattleMap _map;
    private Ability _damage;

    public EnemyDog(BattleMap map, BattleStats battleStats, IDamageCalculation damageCalculation)
        : base(BattleObjectSide.Enemy, battleStats, damageCalculation)
    {
        _map = map;
    }

    public void SetDamageAbility(Ability damage)
    {
        _damage = damage;
    }

    public override async void PlayTurn()
    {
        var players = _map
            .GetBattleObjectsInRadius(this, Math.Max(_map.Height, _map.Width), BattleObjectSide.Ally)
            .Select(x => (BattleCharacter) x);
        var nearestPlayer = SearchNearestPlayer(players);
        if (TryAttack(nearestPlayer)) return;
        await Move(GetOptimalCoordinateToMove(nearestPlayer));
        TryAttack(nearestPlayer);
    }

    private bool TryAttack(BattleCharacter nearestPlayer)
    {
        if (_map.GetStraightDistance(this, nearestPlayer) > 1) 
            return false;
        _damage.Use(nearestPlayer, Stats);
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

        return nearestEnemy;
    }

    private Vector2Int GetOptimalCoordinateToMove(BattleCharacter player)
    {
        var playerCoordinate = _map.GetCoordinate(player);
        var coordinate = _map.GetCoordinate(this);
        var shiftX = coordinate.x;
        var shiftY = coordinate.y;
        var step = Math.Min(Stats.Movement, _map.GetStraightDistance(this, player));
        var countStep = 0;
        while (shiftX >= -1 && shiftX < _map.Height && countStep != step &&
               shiftY >= -1 && shiftY < _map.Height && countStep != step)
        {
            if (playerCoordinate.y != shiftY)
            {
                if (playerCoordinate.y < shiftY && IsEmptyCell(shiftX, shiftY - 1))
                    shiftY -= 1;
                else if (playerCoordinate.y > shiftY && IsEmptyCell(shiftX, shiftY + 1))
                    shiftY += 1;
            }

            if (playerCoordinate.x != shiftX)
            {
                if (playerCoordinate.x < shiftX && IsEmptyCell(shiftX - 1, shiftY))
                    shiftX -= 1;
                else if (playerCoordinate.x > shiftX && IsEmptyCell(shiftX + 1, shiftY))
                    shiftX += 1;
            }

            countStep++;
        }

        return new Vector2Int(shiftX, shiftY);
    }

    private async UniTask Move(Vector2Int coordinate)
    {
        await _map.MoveTo(this, coordinate.x, coordinate.y);
    }

    private bool IsValidCoordinate(int x, int y)
    {
        return x >= 0 && y >= 0 && x <= _map.Height && x <= _map.Width;
    }

    private bool IsEmptyCell(int x, int y)
    {
        var objectOnCoordinate = _map.GetCell(x, y).GetObject();
        return objectOnCoordinate is NullBattleObject;
    }
}