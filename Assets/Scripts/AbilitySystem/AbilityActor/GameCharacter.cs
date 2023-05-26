using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class GameCharacter
    {
        public readonly IBattleCharacterData CharacterData;//View, AbilityData[]
        public readonly BattleStats BattleStats;
        //public readonly EntityType EntityType;

        //=> CharacterData.GetActiveAbilities() + Equipment.Abilities
        public readonly List<ActiveAbilityData> PosessedActiveAbilities = new List<ActiveAbilityData>(); 
        //Equipment

        public GameCharacter(IBattleCharacterData entityData, ActiveAbilityData[] activeAbilities, ActiveAbilityData[] passiveAbilities)
        {
            CharacterData = entityData;
            BattleStats = new BattleStats(entityData.BaseStats);
            //EntityType = entityData.EntityType;
            PosessedActiveAbilities = activeAbilities.ToList();
        }
    }
}
