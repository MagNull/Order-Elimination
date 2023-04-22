using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleMap
    {
        public CellRangeBorders CellRangeBorders { get; }

        public event Action<Vector2Int> CellChanged;
        public bool Contains(IAbilitySystemActor entity);
        public void PlaceEntity(IAbilitySystemActor entity, Vector2Int position);//Check if entity is already on the map
        public void RemoveEntity(IAbilitySystemActor entity);
        public Vector2Int GetPosition(IAbilitySystemActor entity);
        public Vector2Int GetPosition(IReadOnlyCell cell);
        //public Vector2Int GetPosition(IBattleObstacle obstacle);
        public IEnumerable<IAbilitySystemActor> GetContainingEntities(Vector2Int position);

        public float GetGameDistanceBetween(Vector2Int posA, Vector2Int posB);
        //public bool Move(IAbilitySystemActor movingEntity, Vector2Int destination);
        public bool HasPathToDestination(IAbilitySystemActor walker, Vector2Int Destination, Predicate<Vector2Int> isPositionAvailable, out Vector2Int[] path); 
        //public IBattleMap GetSideRepresentation(side)
    }
}
