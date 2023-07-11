using System;
using System.Collections.Generic;
using System.Linq;
using GameInventory;
using OrderElimination.AbilitySystem;
using UnityEngine;

namespace OrderElimination.MacroGame
{
    public class GameCharacter
    {
        private GameCharacterStats _characterStats;
        private float _currentHealth;
        private readonly List<IActiveAbilityData> _activeAbilities = new();
        private readonly List<IPassiveAbilityData> _passiveAbilities = new();

        public IGameCharacterTemplate CharacterData { get; }
        public IReadOnlyList<IActiveAbilityData> ActiveAbilities => _activeAbilities;
        public IReadOnlyList<IPassiveAbilityData> PassiveAbilities => _passiveAbilities;
        public IReadOnlyGameCharacterStats CharacterStats => _characterStats;
        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("Attempt to set health lower than 0");
                _currentHealth = Mathf.Clamp(value, 0, _characterStats.MaxHealth);
                StatsChanged?.Invoke(this);
            }
        }
        public Inventory Inventory { get; private set; } = new(2);

        public event Action<GameCharacter> StatsChanged;

        #region Constructors
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
                specifiedStats.Attack,
                specifiedStats.Accuracy,
                specifiedStats.Evasion,
                specifiedStats.MaxMovementDistance);
            _activeAbilities = activeAbilities.ToList();
            _passiveAbilities = passiveAbilities.ToList();
        }
        #endregion

        public void ChangeStat(BattleStat battleStat, float value)
        {
            _characterStats[battleStat] = value;
            StatsChanged?.Invoke(this);
        }
    }
}
