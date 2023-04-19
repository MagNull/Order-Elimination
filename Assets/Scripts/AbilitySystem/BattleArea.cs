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
        public PointRelativePattern AreaPattern { get; set; }
        public IEnumerable<IAbilitySystemActor> EntitiesInArea => _enitiesInArea;
        public event Action<IAbilitySystemActor> EntityLeavedArea;
        public event Action<IAbilitySystemActor> EntityEnteredArea;

        private HashSet<IAbilitySystemActor> _enitiesInArea;

        public void UpdateArea(IBattleMap battleMap, Vector2Int areaOrigin)
        {
            var currentEntities = AreaPattern
                .GetAbsolutePositions(areaOrigin)
                .SelectMany(pos => battleMap.GetContainingEntities(pos))
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
