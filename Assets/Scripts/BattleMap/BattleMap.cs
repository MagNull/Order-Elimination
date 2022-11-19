using System.Collections.Generic;
using UnityEngine;
using System;
using OrderElimination.BattleMap;

public class BattleMap : MonoBehaviour
{
    public event Action<Cell> CellChanged;

    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    private Cell[,] _cellGrid;

    private Dictionary<IBattleObject, Vector2Int> _destroyedObjectsCoordinates = new();

    public int Width => _width;

    public int Height => _height;

    public void Init(Cell[,] modelGrid)
    {
        _cellGrid = modelGrid;
    }

    public Cell GetCell(int x, int y)
    {
        return _cellGrid[x, y];
    }

    // нужен ли метод?
    public Cell GetCell(IBattleObject battleObject)
    {
        for (var i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (_cellGrid[i, j].GetObject() == battleObject)
                {
                    return _cellGrid[i, j];
                }
            }
        }

        throw new ArgumentException("BattleObject not found");
    }

    public void SetCell(int x, int y, IBattleObject obj)
    {
        if(obj is NullBattleObject or null)
            throw new ArgumentException($"Try to set null battle object in cell ({x},{y})");
        _cellGrid[x, y].SetObject(obj);
        CellChanged?.Invoke(_cellGrid[x, y]);
    }

    public void DestroyObject(IBattleObject battleObject)
    {
        var cell = GetCell(battleObject);
        var boPos = GetCoordinate(battleObject);
        _destroyedObjectsCoordinates.Add(battleObject, new Vector2Int(boPos.x, boPos.y));
        cell.SetObject(new NullBattleObject());
        CellChanged?.Invoke(cell);
    }

    public void MoveTo(IBattleObject obj, int x, int y)
    {
        Vector2Int objCrd = GetCoordinate(obj);
        _cellGrid[objCrd.x, objCrd.y].SetObject(new NullBattleObject());
        SetCell(x, y, obj);
    }

    public int GetDistance(IBattleObject obj1, IBattleObject obj2)
    {
        Vector2Int obj1Crd = GetCoordinate(obj1);
        Vector2Int obj2Crd = GetCoordinate(obj2);
        return Math.Abs(obj1Crd.x - obj2Crd.x) + Math.Abs(obj1Crd.y - obj2Crd.y) - 1;
    }

    public Vector2Int GetCoordinate(IBattleObject obj)
    {
        for (var i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (_cellGrid[i, j].GetObject() == obj)
                {
                    return new Vector2Int(i, j);
                }
            }
        }
        if(_destroyedObjectsCoordinates.TryGetValue(obj, out var coordinates))
            return coordinates;

        Debug.LogError("Объект не найден на поле!");
        return new Vector2Int(-1, -1);
    }

    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius, BattleObjectSide side)
    {
        return GetObjectsInRadius(obj, radius, battleObject =>
            battleObject is not NullBattleObject && battleObject.Side == side);
    }

    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius)
    {
        return GetObjectsInRadius(obj, radius, battleObject =>
            battleObject is not NullBattleObject);
    }

    public IList<IBattleObject> GetEmptyObjectsInRadius(IBattleObject obj, int radius)
    {
        return GetObjectsInRadius(obj, radius, battleObject => battleObject is NullBattleObject);
    }

    //private void OnCellClicked(CellView cellView) => CellClicked?.Invoke(cellView);

    private IList<IBattleObject> GetObjectsInRadius(IBattleObject obj, int radius, Predicate<IBattleObject> predicate)
    {
        Vector2Int objCrd = GetCoordinate(obj);
        List<IBattleObject> objects = new List<IBattleObject>();
        for (var i = objCrd.x - radius; i <= objCrd.x + radius; i++)
        {
            for (var j = objCrd.y - radius; j <= objCrd.y + radius; j++)
            {
                if (i >= 0 && i < _cellGrid.GetLength(0) && j >= 0 && j < _cellGrid.GetLength(1))
                {
                    if (predicate(_cellGrid[i, j].GetObject()))
                    {
                        objects.Add(_cellGrid[i, j].GetObject());
                    }
                }
            }
        }

        objects.Remove(obj);
        return objects;
    }
}