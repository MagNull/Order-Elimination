﻿using System;
using System.Collections.Generic;
using System.Linq;
using OrderElimination.AbilitySystem;

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
                if (value > CharacterStats.MaxHealth)
                    throw new ArgumentOutOfRangeException("Attempt to set health higher than MaxHealth");
                _currentHealth = value;
                StatsChanged?.Invoke(this);
            }
        }
        public Inventory_Items.Inventory Inventory { get; private set; } = new(2);

        public event Action<GameCharacter> StatsChanged;

        #region Constructors
        [Obsolete("Use " + nameof(GameCharactersFactory) + " instead.")]
        public GameCharacter(
            IGameCharacterTemplate template,
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities)
            : this(template, activeAbilities, passiveAbilities, template.GetBaseBattleStats()) { }

        [Obsolete("Use " + nameof(GameCharactersFactory) + " instead.")]
        public GameCharacter(
            IGameCharacterTemplate template,
            IEnumerable<IActiveAbilityData> activeAbilities,
            IEnumerable<IPassiveAbilityData> passiveAbilities,
            IReadOnlyGameCharacterStats specifiedStats)
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
