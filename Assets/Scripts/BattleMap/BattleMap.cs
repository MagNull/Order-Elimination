using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using OrderElimination.BattleMap;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class BattleMap : MonoBehaviour
{
    public event Action<Cell> CellChanged;

    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;

    private Cell[,] _cellGrid;

    private readonly Dictionary<IBattleObject, Vector2Int> _destroyedObjectsCoordinates = new();
    private readonly Dictionary<Vector2Int, EnvironmentObject> _activeEnvironmentObjects = new();

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

    public void DestroyObject(IBattleObject battleObject)
    {
        var boPos = GetCoordinate(battleObject);
        _destroyedObjectsCoordinates.Add(battleObject, new Vector2Int(boPos.x, boPos.y));
        SetCell(boPos.x, boPos.y, new NullBattleObject());
    }

    public int GetStraightDistance(IBattleObject obj1, IBattleObject obj2)
    {
        Vector2Int obj1Crd = GetCoordinate(obj1);
        Vector2Int obj2Crd = GetCoordinate(obj2);
        return Math.Abs(obj1Crd.x - obj2Crd.x) + Math.Abs(obj1Crd.y - obj2Crd.y);
    }

    public int GetPathDistance(IBattleObject obj1, IBattleObject obj2)
    {
        var target = GetCoordinate(obj2);
        var path = GetShortestPath(obj1, target.x, target.y);
        return path.Count;
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

        if (_destroyedObjectsCoordinates.TryGetValue(obj, out var coordinates))
            return coordinates;

        Debug.LogWarning($"$Объект {obj.View.name} не найден на поле!");
        return new Vector2Int(-1, -1);
    }

    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius, BattleObjectSide side)
    {
        return GetObjectsInRadius(obj, radius, battleObject =>
            battleObject is not NullBattleObject && battleObject.Side == side);
    }

    public bool GetCellWith(Func<Cell, bool> predicate, out Cell cell)
    {
        for (var i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (!predicate(_cellGrid[i, j])) continue;
                cell = _cellGrid[i, j];
                return true;
            }
        }

        cell = null;
        return false;
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

    public async UniTask MoveTo(IBattleObject obj, int x, int y, float delay = -1)
    {
        Vector2Int objCoord = GetCoordinate(obj);
        if (objCoord != new Vector2Int(-1, -1))
        {
            obj.OnMoved(GetCell(obj), GetCell(x, y));
            if (_activeEnvironmentObjects.ContainsKey(objCoord))
            {
                SetCell(objCoord.x, objCoord.y, _activeEnvironmentObjects[objCoord]);
                _activeEnvironmentObjects[objCoord].OnLeave(obj);
                _activeEnvironmentObjects.Remove(objCoord);
            }
            else
                SetCell(objCoord.x, objCoord.y, new NullBattleObject());
        }

        SetCell(x, y, obj);
        await UniTask.Delay(TimeSpan.FromSeconds(delay == -1 ? 0 : delay));
    }

    public List<Vector2Int> GetShortestPath(IBattleObject obj, int x, int y)
    {
        var path = new List<Vector2Int>();
        var objCoord = GetCoordinate(obj);
        var target = new Vector2Int(x, y);
        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        var parents = new Dictionary<Vector2Int, Vector2Int>();
        queue.Enqueue(objCoord);
        visited.Add(objCoord);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == target)
            {
                while (current != objCoord)
                {
                    path.Add(current);
                    current = parents[current];
                }

                path.Reverse();
                return path;
            }

            foreach (var neighbour in GetNeighbours(current))
            {
                if (visited.Contains(neighbour) ||
                    GetCell(neighbour.x, neighbour.y).GetObject().Side != BattleObjectSide.None)
                    continue;

                visited.Add(neighbour);
                queue.Enqueue(neighbour);
                parents.Add(neighbour, current);
            }
        }

        Debug.Log("Path len: " + path.Count);
        return path;
    }

    private List<Vector2Int> GetNeighbours(Vector2Int coord)
    {
        var neighbours = new List<Vector2Int>();
        if (coord.x > 0)
            neighbours.Add(new Vector2Int(coord.x - 1, coord.y));
        if (coord.x < _width - 1)
            neighbours.Add(new Vector2Int(coord.x + 1, coord.y));
        if (coord.y > 0)
            neighbours.Add(new Vector2Int(coord.x, coord.y - 1));
        if (coord.y < _height - 1)
            neighbours.Add(new Vector2Int(coord.x, coord.y + 1));
        if (coord.y < _height - 1 && coord.x < _width - 1)
            neighbours.Add(new Vector2Int(coord.x + 1, coord.y + 1));
        if (coord.y > 0 && coord.x > 0)
            neighbours.Add(new Vector2Int(coord.x - 1, coord.y - 1));
        if (coord.y > 0 && coord.x < _width - 1)
            neighbours.Add(new Vector2Int(coord.x + 1, coord.y - 1));
        if (coord.y < _height - 1 && coord.x > 0)
            neighbours.Add(new Vector2Int(coord.x - 1, coord.y + 1));
        return neighbours;
    }

    private Vector2Int GetCellCoordinate(Cell cell)
    {
        for (var i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (_cellGrid[i, j] == cell)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        throw new ArgumentException("Cell not found");
    }

    private void SetCell(int x, int y, IBattleObject obj)
    {
        if (obj is null)
            throw new ArgumentException($"Try to set null in cell ({x},{y})");
        if (_cellGrid[x, y].GetObject() is EnvironmentObject environmentObject)
        {
            environmentObject.OnEnter(obj);
            _activeEnvironmentObjects.Add(new Vector2Int(x, y), environmentObject);
        }

        _cellGrid[x, y].SetObject(obj);
        CellChanged?.Invoke(_cellGrid[x, y]);
    }

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