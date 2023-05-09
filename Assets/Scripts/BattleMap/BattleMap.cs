using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using OrderElimination.BM;
using Sirenix.Utilities;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using OrderElimination.Infrastructure;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Infrastructure;

public class BattleMap : MonoBehaviour, IBattleMap
{
    #region Refactored for IBattleMap
    private Dictionary<AbilitySystemActor, Vector2Int> _containedEntitiesPositions;
    private Dictionary<IReadOnlyCell, Vector2Int> _cellCoordinates;

    public CellRangeBorders CellRangeBorders { get; private set; }

    public event Action<Vector2Int> CellChanged;

    public IEnumerable<AbilitySystemActor> GetContainedEntities(Vector2Int position)
    {
        if (!CellRangeBorders.Contains(position))
            throw new ArgumentOutOfRangeException();
        return GetCell(position.x, position.y).GetContainingEntities();
    }

    public Vector2Int GetPosition(AbilitySystemActor entity)
    {
        if (!_containedEntitiesPositions.ContainsKey(entity))
            throw new ArgumentException("Entity does not exist on the map.");
        return _containedEntitiesPositions[entity];
    }

    public Vector2Int GetPosition(IReadOnlyCell cell) => _cellCoordinates[cell];

    public bool Contains(AbilitySystemActor entity)
        => _containedEntitiesPositions.ContainsKey(entity);

    public void PlaceEntity(AbilitySystemActor entity, Vector2Int position)
    {
        if (_containedEntitiesPositions.ContainsKey(entity))
            throw new InvalidCastException("Entity already exists on the map.");
        _containedEntitiesPositions.Add(entity, position);
        GetCell(position.x, position.y).AddEntity(entity);
        CellChanged?.Invoke(position);
    }

    public void RemoveEntity(AbilitySystemActor entity)
    {
        if (!_containedEntitiesPositions.ContainsKey(entity))
            throw new InvalidCastException("Entity does not exist on the map.");
        var position = _containedEntitiesPositions[entity];
        GetCell(position.x, position.y).RemoveEntity(entity);
        _containedEntitiesPositions.Remove(entity);
        CellChanged?.Invoke(position);
    }

    public float GetGameDistanceBetween(Vector2Int posA, Vector2Int posB)
        => CellMath.GetRealDistanceBetween(posA, posB);

    public bool PathExists(Vector2Int origin, Vector2Int destination, Predicate<Vector2Int> positionPredicate, out Vector2Int[] path)
    {
        var result = new List<Vector2Int>();
        var visited = new HashSet<Vector2Int>();
        var queue = new Queue<Vector2Int>();
        var parents = new Dictionary<Vector2Int, Vector2Int>();
        queue.Enqueue(origin);
        visited.Add(origin);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (current == destination)
            {
                while (current != origin)
                {
                    result.Add(current);
                    current = parents[current];
                }
                result.Reverse();
                path = result.ToArray();
                return true;
            }

            foreach (var neighbour in GetNeighbours(current))
            {
                if (visited.Contains(neighbour) || !positionPredicate(neighbour))
                    continue;

                visited.Add(neighbour);
                queue.Enqueue(neighbour);
                parents.Add(neighbour, current);
            }
        }
        path = result.ToArray();
        return false;
    }

    #endregion

    public event Action<Cell, bool> CellChangedOld;

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

    public void Init(Cell[,] modelGrid)// Ц Width, Height ??
    {
        _cellGrid = modelGrid;
        CellRangeBorders = new CellRangeBorders(0, 0, Width - 1, Height - 1);
        _containedEntitiesPositions = new Dictionary<AbilitySystemActor, Vector2Int>();
        _cellCoordinates = new Dictionary<IReadOnlyCell, Vector2Int>();
        for (var x = 0; x < _cellGrid.GetLength(0); x++)
        {
            for (var y = 0; y < _cellGrid.GetLength(1); y++)
            {
                _cellCoordinates.Add(_cellGrid[x, y], new Vector2Int(x, y));
            }
        }
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

    public Cell GetCell(IBattleObject battleObject)
    {
        for (var x = 0; x < _cellGrid.GetLength(0); x++)
        {
            for (var y = 0; y < _cellGrid.GetLength(1); y++)
            {
                if (_cellGrid[x, y].Objects.Contains(battleObject))
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
        SetCell(boPos.x, boPos.y, new NullBattleObject(), true);
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
                if (_cellGrid[i, j].Objects.Contains(obj))
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        if (_destroyedObjectsCoordinates.TryGetValue(obj, out var coordinates))
            return coordinates;

        if (obj is EnvironmentObject env && _activeEnvironmentObjects.ContainsValue(env))
            return _activeEnvironmentObjects.FirstOrDefault(x => x.Value == env).Key;

        Debug.LogWarning($"$ќбъект {obj.View.GameObject.name} не найден на поле!");
        return new Vector2Int(-1, -1);
    }

    public IList<IBattleObject> GetBattleObjectsInPatternArea(IBattleObject obj, IBattleObject source,
        Vector2Int[] pattern, BattleObjectType type = BattleObjectType.None, int maxDistance = 999)
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
                    type == BattleObjectType.None || cell.Objects.Any(x => x.Type == type))
                {
                    result.AddRange(cell.Objects);
                }
            }
        }

        return result;
    }

    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius,
        BattleObjectType type = BattleObjectType.None)
    {
        return GetObjectsInRadius(obj, radius,
            battleObject => (type == BattleObjectType.None || battleObject.Type == type));
    }

    public IList<IBattleObject> GetEmptyObjectsInRadius(IBattleObject obj, int radius)
    {
        return GetObjectsInRadius(obj, radius, battleObject => battleObject is NullBattleObject, true);
    }

    public void SpawnObject(IBattleObject obj, int x, int y)
    {
        SetCell(x, y, obj, false);
        _cellGrid[x, y].AddObject(obj);
    }

    //TODO: Continue refactoring
    public async UniTask MoveTo(IBattleObject obj, int x, int y, float delay = -1)
    {
        var exist = false;
        for (var i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (_cellGrid[i, j].Objects.Contains(obj))
                {
                    exist = true;
                    break;
                }
            }
        }

        if (!exist)
            return;


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
                SetCell(objCoord.x, objCoord.y, _activeEnvironmentObjects[objCoord], false);
                _activeEnvironmentObjects[objCoord].OnLeave(obj);
                _activeEnvironmentObjects.Remove(objCoord);
            }
            else
                SetCell(objCoord.x, objCoord.y, new NullBattleObject(), false);
        }

        SetCell(x, y, obj, true);
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
                    neighbourCell.Objects.Any(obj => obj is BattleCharacter ||
                                                     obj.Type == BattleObjectType.Obstacle))
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

    private void SetCell(int x, int y, IBattleObject obj, bool tween)
    {
        if (x < 0 || x > _width - 1 || y < 0 || y > _height - 1)
            throw new ArgumentException($"Cell ({x}, {y}) not found");
        if (obj is null)
            throw new ArgumentException($"Try to set null in cell ({x},{y})");
        if (obj is BattleCharacter &&
            _cellGrid[x, y].Objects.Skip(1).All(o => o is EnvironmentObject environmentObject))
        {
            if (_cellGrid[x, y].Objects.FirstOrDefault(o => o is EnvironmentObject) is EnvironmentObject
                environmentObject)
            {
                environmentObject.OnEnter(obj);
                _activeEnvironmentObjects.Add(new Vector2Int(x, y), environmentObject);
            }
        }

        if (tween)
        {
            var oldCell = GetCell(obj);
            oldCell.RemoveObject(obj);
            CellChangedOld?.Invoke(oldCell, tween);
        }

        _cellGrid[x, y].AddObject(obj);
        CellChangedOld?.Invoke(_cellGrid[x, y], tween);
    }

    private IList<IBattleObject> GetObjectsInRadius(IBattleObject obj, int radius, Predicate<IBattleObject> predicate,
        bool predicateForAll = false)

    {
        Vector2Int objCrd = GetCoordinate(obj);
        List<IBattleObject> objects = new List<IBattleObject>();
        for (var i = objCrd.x - radius; i <= objCrd.x + radius; i++)
        {
            for (var j = objCrd.y - radius; j <= objCrd.y + radius; j++)
            {
                if (i < 0 || i >= _cellGrid.GetLength(0) || j < 0 || j >= _cellGrid.GetLength(1))
                    continue;
                var objectsToAdd = _cellGrid[i, j].Objects.Where(x => predicate(x)).ToList();
                switch (predicateForAll)
                {
                    case true when objectsToAdd.Count == _cellGrid[i, j].Objects.Count:
                        objects.AddRange(objectsToAdd);
                        break;
                    case false when objectsToAdd.Any():
                        objects.Add(objectsToAdd.First());
                        break;
                }
            }
        }

        objects.Remove(obj);
        return objects;
    }
}