using System.Collections.Generic;
using UnityEngine;
using System;
using OrderElimination;
using OrderElimination.Infrastructure;
using OrderElimination.AbilitySystem;

public class BattleMap : MonoBehaviour, IBattleMap
{
    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    private Cell[,] _cellGrid;
    private Dictionary<AbilitySystemActor, Vector2Int> _containedEntitiesPositions;
    private Dictionary<AbilitySystemActor, Vector2Int> _lastEntitiesPositions;
    private Dictionary<IReadOnlyCell, Vector2Int> _cellCoordinates;

    public int Width { get; private set; }
    public int Height { get; private set; }
    public CellRangeBorders CellRangeBorders { get; private set; }

    public event Action<Vector2Int> CellChanged;
    public event Action<AbilitySystemActor> PlacedOnMap;
    public event Action<AbilitySystemActor> RemovedFromMap;

    public IEnumerable<AbilitySystemActor> GetContainedEntities(Vector2Int position)
    {
        if (!CellRangeBorders.Contains(position))
            Logging.LogException(new ArgumentOutOfRangeException());
        return GetCell(position.x, position.y).GetContainingEntities();
    }

    public bool ContainsEntity(AbilitySystemActor entity)
        => _containedEntitiesPositions.ContainsKey(entity);

    public bool ContainsPosition(Vector2Int position)
        => CellRangeBorders.Contains(position);

    public Vector2Int GetPosition(AbilitySystemActor entity)
    {
        if (!ContainsEntity(entity))
            throw new ArgumentException("Entity does not exist on the map.");
        return _containedEntitiesPositions[entity];
    }

    public Vector2Int GetLastPosition(AbilitySystemActor entity)
    {
        if (!ContainsEntity(entity) && !_lastEntitiesPositions.ContainsKey(entity))
        {
            Logging.LogException(new ArgumentException("Entity never existed on the map."));
        }
        return _lastEntitiesPositions[entity];
    }

    public Vector2Int GetPosition(IReadOnlyCell cell) => _cellCoordinates[cell];

    public void PlaceEntity(AbilitySystemActor entity, Vector2Int position)
    {
        if (!_containedEntitiesPositions.ContainsKey(entity))
        {
            //place first time
            _containedEntitiesPositions.Add(entity, position);
            _lastEntitiesPositions.Add(entity, position);
            GetCell(position.x, position.y).AddEntity(entity);
            PlacedOnMap?.Invoke(entity);
            CellChanged?.Invoke(position);
        }
        else
        {
            //move
            var oldPos = _containedEntitiesPositions[entity];
            _containedEntitiesPositions[entity] = position;
            _lastEntitiesPositions[entity] = position;
            GetCell(oldPos.x, oldPos.y).RemoveEntity(entity);
            GetCell(position.x, position.y).AddEntity(entity);
            CellChanged?.Invoke(oldPos);
            CellChanged?.Invoke(position);
        }
    }

    public void RemoveEntity(AbilitySystemActor entity)
    {
        if (!_containedEntitiesPositions.ContainsKey(entity))
            Logging.LogException( new InvalidCastException("Entity does not exist on the map."));
        var position = _containedEntitiesPositions[entity];
        GetCell(position.x, position.y).RemoveEntity(entity);
        _containedEntitiesPositions.Remove(entity);
        RemovedFromMap?.Invoke(entity);
        CellChanged?.Invoke(position);
    }

    public float GetGameDistanceBetween(Vector2Int posA, Vector2Int posB)
        => CellMath.GetRealDistanceBetween(posA, posB);

    public bool PathExists(
        Vector2Int origin, Vector2Int destination, Predicate<Vector2Int> positionPredicate, out Vector2Int[] path)
    {
        return Pathfinding.PathExists(origin, destination, CellRangeBorders, positionPredicate, out path);
    }

    public void Init(Cell[,] modelGrid)
    {
        _cellGrid = modelGrid;
        Width = _cellGrid.GetLength(0);
        Height = _cellGrid.GetLength(1);
        CellRangeBorders = new CellRangeBorders(0, 0, Width - 1, Height - 1);
        _containedEntitiesPositions = new();
        _lastEntitiesPositions = new();
        _cellCoordinates = new Dictionary<IReadOnlyCell, Vector2Int>();
        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                _cellCoordinates.Add(_cellGrid[x, y], new Vector2Int(x, y));
            }
        }
    }

    private Cell GetCell(int x, int y)
    {
        if (x < 0 || x > Width - 1 || y < 0 || y > Height - 1)
            Logging.LogException(new ArgumentException($"Cell ({x}, {y}) not found"));
        return _cellGrid[x, y];
    }
}