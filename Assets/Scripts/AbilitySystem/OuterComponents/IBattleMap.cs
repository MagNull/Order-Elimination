using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleMap
    {
        public CellRangeBorders CellRangeBorders { get; }
        
        public event Action<Vector2Int> CellChanged;
        public event Action<AbilitySystemActor> PlacedOnMap;
        public event Action<AbilitySystemActor> RemovedFromMap;

        public bool ContainsEntity(AbilitySystemActor entity);
        public void PlaceEntity(AbilitySystemActor entity, Vector2Int position);
        public void RemoveEntity(AbilitySystemActor entity);
        public bool ContainsPosition(Vector2Int position);
        public Vector2Int GetPosition(AbilitySystemActor entity);
        public Vector2Int GetPosition(IReadOnlyCell cell);
        public IEnumerable<AbilitySystemActor> GetContainedEntities(Vector2Int position);
        public float GetGameDistanceBetween(Vector2Int posA, Vector2Int posB);
        public bool PathExists(Vector2Int origin, Vector2Int destination, Predicate<Vector2Int> isPositionAvailable, out Vector2Int[] path);
        //public IBattleMap GetSideRepresentation(side)
    }
}
