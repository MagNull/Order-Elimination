using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using OrderElimination.BM;
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
    [SerializeField]
    private SerializedDictionary<Vector2Int, EnvironmentObject> _activeEnvironmentObjects = new();

    public int Width => _width;

    public int Height => _height;

    public void Init(Cell[,] modelGrid)
    {
        _cellGrid = modelGrid;
    }

    public bool ExistCoordinate(Vector2Int point)
    {
        return point.x >= 0 && point.x < _width && point.y >= 0 && point.y < _height;
    }

    public Cell GetCell(int x, int y)
    {
        if (x < 0 || x > _width - 1 || y < 0 || y > _height - 1)
            throw new ArgumentException($"Cell ({x}, {y}) not found");
        return _cellGrid[x, y];
    }

    public Cell GetCell(Vector2Int point)
    {
        if (point.x < 0 || point.x > _width - 1 || point.y < 0 || point.y > _height - 1)
            throw new ArgumentException($"Cell ({point.x}, {point.y}) not found");
        return _cellGrid[point.x, point.y];
    }

    public Cell GetCell(IBattleObject battleObject)
    {
        for (var x = 0; x < _cellGrid.GetLength(0); x++)
        {
            for (var y = 0; y < _cellGrid.GetLength(1); y++)
            {
                if (_cellGrid[x, y].GetObject() == battleObject)
                {
                    return _cellGrid[x, y];
                }

                if (_activeEnvironmentObjects.ContainsKey(new Vector2Int(x, y)))
                {
                    return GetCell(x, y);
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
        if (battleObject is EnvironmentObject && _activeEnvironmentObjects.ContainsKey(boPos))
        {
            _activeEnvironmentObjects.Remove(boPos);
        }
    }

    public int GetStraightDistance(IBattleObject obj1, IBattleObject obj2)
    {
        Vector2Int obj1Crd = GetCoordinate(obj1);
        Vector2Int obj2Crd = GetCoordinate(obj2);
        return Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(obj1Crd.x - obj2Crd.x, 2) +
                                           Mathf.Pow(obj1Crd.y - obj2Crd.y, 2)));
    }

    public int GetStraightDistance(IBattleObject obj1, Vector2Int pos)
    {
        Vector2Int obj1Crd = GetCoordinate(obj1);
        return Mathf.FloorToInt(Mathf.Sqrt(Mathf.Pow(obj1Crd.x - pos.x, 2) +
                                           Mathf.Pow(obj1Crd.y - pos.y, 2)));
    }

    public int GetPathDistance(IBattleObject origin, IBattleObject destination)
    {
        var destinationCrd = GetCoordinate(destination);
        var path = GetShortestPath(origin, destinationCrd.x, destinationCrd.y);
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

        if (obj is EnvironmentObject env && _activeEnvironmentObjects.ContainsValue(env))
            return _activeEnvironmentObjects.FirstOrDefault(x => x.Value == env).Key;

        Debug.LogWarning($"$������ {obj.View.GameObject.name} �� ������ �� ����!");
        return new Vector2Int(-1, -1);
    }

    public IList<IBattleObject> GetBattleObjectsInPatternArea(IBattleObject obj, IBattleObject source,
        Vector2Int[] pattern, BattleObjectSide side = BattleObjectSide.None, int maxDistance = 999)
    {
        var center = GetCoordinate(obj);
        var sourcePos = GetCoordinate(source);
        var directionVector = center - sourcePos;
        var angle = Vector2.Angle(directionVector, Vector2.right);

        var result = new List<IBattleObject>();
        foreach (var patternElement in pattern)
        {
            var patternElementRotated = Quaternion.FromToRotation(Vector2.right, (Vector2) directionVector)
                                        * (Vector2) patternElement;

            var newPoint = new Vector2Int(center.x + Mathf.RoundToInt(patternElementRotated.x),
                center.y + Mathf.RoundToInt(patternElementRotated.y));
            if ((newPoint - sourcePos).magnitude > maxDistance)
            {
                newPoint = sourcePos +
                           Vector2Int.RoundToInt(Vector2.ClampMagnitude(newPoint - sourcePos, maxDistance));
            }

            if (ExistCoordinate(newPoint))
            {
                var cell = GetCell(newPoint.x, newPoint.y);
                if (
                    (side == BattleObjectSide.None || cell.GetObject().Side == side))
                {
                    result.Add(cell.GetObject());
                }
            }
        }

        return result;
    }

    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius,
        BattleObjectSide side = BattleObjectSide.None)
    {
        return GetObjectsInRadius(obj, radius,
            battleObject => (side == BattleObjectSide.None || battleObject.Side == side));
    }

    public IList<IBattleObject> GetEmptyObjectsInRadius(IBattleObject obj, int radius)
    {
        return GetObjectsInRadius(obj, radius, battleObject => battleObject is NullBattleObject);
    }

    public async UniTask MoveTo(IBattleObject obj, int x, int y, float delay = -1)
    {
        Vector2Int objCoord = GetCoordinate(obj);
        //TODO: Fix environment move
        if (obj is EnvironmentObject && _activeEnvironmentObjects.ContainsKey(new Vector2Int(x, y)))
        {
            obj.View.Disable();
            return;
        }

        if (objCoord != new Vector2Int(-1, -1) && obj is BattleCharacter)
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
        if (delay <= 0)
            return;
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
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
                var neighbourCell = GetCell(neighbour.x, neighbour.y);
                if (visited.Contains(neighbour) ||
                    neighbourCell.GetObject() is BattleCharacter ||
                    neighbourCell.GetObject().Side == BattleObjectSide.Obstacle)
                    continue;

                visited.Add(neighbour);
                queue.Enqueue(neighbour);
                parents.Add(neighbour, current);
            }
        }

        return path;
    }

    public Vector2Int GetOptimalPosition(IBattleObject origin, IBattleObject target, int radius, int optimalDistance)
    {
        var availableCells = GetEmptyObjectsInRadius(origin, radius);
        IBattleObject optimalPos = null;
        IBattleObject closestPos = origin;
        foreach (var pos in availableCells)
        {
            var distance = GetStraightDistance(pos, target);
            if (distance == optimalDistance)
            {
                optimalPos = pos;
            }

            if (distance < GetStraightDistance(closestPos, target))
            {
                closestPos = pos;
            }
        }

        if (optimalPos == null)
            return GetCoordinate(closestPos);
        return GetCoordinate(optimalPos);
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

    private void SetCell(int x, int y, IBattleObject obj)
    {
        if (x < 0 || x > _width - 1 || y < 0 || y > _height - 1)
            throw new ArgumentException($"Cell ({x}, {y}) not found");
        if (obj is null)
            throw new ArgumentException($"Try to set null in cell ({x},{y})");
        if (obj is BattleCharacter && _cellGrid[x, y].GetObject() is EnvironmentObject environmentObject)
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