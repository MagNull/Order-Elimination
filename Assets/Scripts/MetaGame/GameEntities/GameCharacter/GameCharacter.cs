using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

namespace OrderElimination.MetaGame
{
    public class GameCharacter
    {
        private GameCharacterStats _characterStats;
        private readonly List<IActiveAbilityData> _activeAbilities = new();
        private readonly List<IPassiveAbilityData> _passiveAbilities = new();

        public readonly IGameCharacterTemplate CharacterData;
        public IReadOnlyList<IActiveAbilityData> ActiveAbilities => _activeAbilities;
        public IReadOnlyList<IPassiveAbilityData> PassiveAbilities => _passiveAbilities;
        public IReadOnlyGameCharacterStats CharacterStats => _characterStats;
        //Equipment
        public Inventory_Items.Inventory Inventory { get; private set; } = new(2);

        [Obsolete("Use " + nameof(GameCharactersFactory) + " instead.")]
        public GameCharacter(
            IGameCharacterTemplate template,
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities)
            : this(template, template.GetBaseBattleStats(), activeAbilities, passiveAbilities) { }

        [Obsolete("Use " + nameof(GameCharactersFactory) + " instead.")]
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
            _activeAbilities = activeAbilities.ToList();
            _passiveAbilities = passiveAbilities.ToList();
        }

        public void ChangeStat(BattleStat battleStat, float value)
        {
            _characterStats[battleStat] = value;
        }
    }
}
