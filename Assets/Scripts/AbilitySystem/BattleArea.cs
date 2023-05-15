using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class BattleArea
    {
        public IPointRelativePattern AreaPattern { get; set; }
        public IEnumerable<AbilitySystemActor> EntitiesInArea => _enitiesInArea;
        public event Action<AbilitySystemActor> EntityLeavedArea;
        public event Action<AbilitySystemActor> EntityEnteredArea;

        private HashSet<AbilitySystemActor> _enitiesInArea;

        public void UpdateArea(IBattleMap battleMap, Vector2Int areaOrigin)
        {
            var currentEntities = AreaPattern
                .GetAbsolutePositions(areaOrigin)
                .SelectMany(pos => battleMap.GetContainedEntities(pos))
                .ToHashSet();
            foreach (var entity in _enitiesInArea)
            {
                if (!currentEntities.Contains(entity))
                {
                    _enitiesInArea.Remove(entity);
                    EntityLeavedArea?.Invoke(entity);
                }
            }
            foreach (var entity in currentEntities)
            {
                if (!_enitiesInArea.Contains(entity))
                {
                    _enitiesInArea.Add(entity);
                    EntityEnteredArea?.Invoke(entity);
                }    
            }
        }
    }
}
