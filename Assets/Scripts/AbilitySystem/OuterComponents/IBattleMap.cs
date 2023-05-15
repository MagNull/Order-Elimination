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
        public bool Contains(AbilitySystemActor entity);
        public void PlaceEntity(AbilitySystemActor entity, Vector2Int position);//Check if entity is already on the map
        public void RemoveEntity(AbilitySystemActor entity);
        public Vector2Int GetPosition(AbilitySystemActor entity);
        public Vector2Int GetPosition(IReadOnlyCell cell);
        //public Vector2Int GetPosition(IBattleObstacle obstacle);
        public IEnumerable<AbilitySystemActor> GetContainedEntities(Vector2Int position);
        public float GetGameDistanceBetween(Vector2Int posA, Vector2Int posB);
        //public bool Move(IAbilitySystemActor movingEntity, Vector2Int destination);
        public bool PathExists(Vector2Int origin, Vector2Int destination, Predicate<Vector2Int> isPositionAvailable, out Vector2Int[] path); 
        //public IBattleMap GetSideRepresentation(side)
    }
}
