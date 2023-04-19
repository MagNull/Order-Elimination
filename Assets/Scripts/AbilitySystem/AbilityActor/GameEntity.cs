using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class GameEntity
    {
        public readonly IBattleEntityInfo EntityData;//View, AbilityData[]
        public readonly BattleStats BattleStats;
        public readonly EntityType EntityType;

        //=> CharacterData.GetActiveAbilities() + Equipment.Abilities
        public readonly List<AbilityData> PosessedActiveAbilities = new List<AbilityData>(); 
        //Equipment

        public GameEntity(IBattleEntityInfo entityData)
        {
            EntityData = entityData;
            BattleStats = new BattleStats(entityData.BaseStats);
            EntityType = entityData.EntityType;
            PosessedActiveAbilities = entityData.GetActiveAbilities().ToList();
        }
    }
}
