using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

namespace OrderElimination.MetaGame
{
    public class GameCharacter
    {
        private GameCharacterStats _characterStats;

        public readonly IGameCharacterTemplate CharacterData;//View, AbilityData[]
        public readonly List<IActiveAbilityData> PosessedActiveAbilities = new(); 
        public readonly List<IPassiveAbilityData> PosessedPassiveAbilities = new();
        public IReadOnlyGameCharacterStats CharacterStats => _characterStats;
        //Equipment
        public Inventory_Items.Inventory Inventory { get; private set; } = new(2);

        public GameCharacter(
            IGameCharacterTemplate template,
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities)
            : this(template, template.GetBaseBattleStats(), activeAbilities, passiveAbilities) { }

        public GameCharacter(
            IGameCharacterTemplate template,
            IReadOnlyGameCharacterStats specifiedStats,
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities)
        {
            CharacterData = template;
            _characterStats = new GameCharacterStats(
                specifiedStats.MaxHealth,
                specifiedStats.MaxArmor,
                specifiedStats.AttackDamage,
                specifiedStats.Accuracy,
                specifiedStats.Evasion,
                specifiedStats.MaxMovementDistance);
            PosessedActiveAbilities = activeAbilities.ToList();
            PosessedPassiveAbilities = passiveAbilities.ToList();
        }

        public void ChangeStat(BattleStat battleStat, float value)
        {
            _characterStats[battleStat] = value;
        }
    }
}
