using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

namespace OrderElimination.MetaGame
{
    public class GameCharacter
    {
        public readonly IGameCharacterData CharacterData;//View, AbilityData[]
        public readonly BattleStats BattleStats;

        public readonly List<IActiveAbilityData> PosessedActiveAbilities = new(); 
        public readonly List<IPassiveAbilityData> PosessedPassiveAbilities = new();
        //Equipment
        public Inventory_Items.Inventory Inventory { get; private set; } = new(2);

        public GameCharacter(
            IGameCharacterData entityData, 
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities)
        {
            CharacterData = entityData;
            BattleStats = new BattleStats(entityData.BaseBattleStats);
            PosessedActiveAbilities = activeAbilities.ToList();
            PosessedPassiveAbilities = passiveAbilities.ToList();
        }

        public void Upgrade(
            float healthMul, float armorMul, float attackMul, float accuracyMul, float evasionMul)
        {
            BattleStats[BattleStat.MaxHealth].UnmodifiedValue *= healthMul;
            BattleStats[BattleStat.MaxArmor].UnmodifiedValue *= armorMul;
            BattleStats[BattleStat.AttackDamage].UnmodifiedValue *= attackMul;
            BattleStats[BattleStat.Accuracy].UnmodifiedValue *= accuracyMul;
            BattleStats[BattleStat.Evasion].UnmodifiedValue *= evasionMul;
        }
    }
}
