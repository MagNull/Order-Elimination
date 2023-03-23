using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class BattleArea
    {
        public AreaPattern Pattern { get; set; }
        public List<IBattleEntity> EntitiesInArea { get; set; }

        public event Action<IBattleEntity> EntityLeavedArea;
        public event Action<IBattleEntity> EntityEnteredArea;

        //TODO Optimize
        public void UpdateArea(BattleMap battleMap)
        {
            var currentEntities = new List<IBattleEntity>();//Pattern.GetEntitiesInArea(battleMap, startingPoint)
            foreach (var entity in EntitiesInArea)
            {
                if (!currentEntities.Contains(entity))
                {
                    EntitiesInArea.Remove(entity);
                    EntityLeavedArea?.Invoke(entity);
                }
            }
            foreach (var entity in currentEntities)
            {
                if (!EntitiesInArea.Contains(entity))
                {
                    EntitiesInArea.Add(entity);
                    EntityEnteredArea?.Invoke(entity);
                }    
            }
        }
    }
}
