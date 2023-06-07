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
        public readonly List<IActiveAbilityData> PosessedActiveAbilities = new(); 
        public readonly List<IPassiveAbilityData> PosessedPassiveAbilities = new(); 
        //Equipment

        public GameCharacter(
            IBattleCharacterData entityData, 
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities)
        {
            CharacterData = entityData;
            BattleStats = new BattleStats(entityData.BaseStats);
            //EntityType = entityData.EntityType;
            PosessedActiveAbilities = activeAbilities.ToList();
            PosessedPassiveAbilities = passiveAbilities.ToList();
        }
    }
}
