using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct PlayerProgressSerializableData
    {
        private readonly Dictionary<GameCurrency, int> _currencies;

        public PlayerProgressSerializableData(
            GameCharacterSaveData[] playerSquadCharacters,
            StrategyStats statsUpgrades,
            IReadOnlyDictionary<GameCurrency, int> currencies)
        {
            PlayerSquadCharacters = playerSquadCharacters;
            StatsUpgrades = statsUpgrades;
            _currencies = currencies.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public GameCharacterSaveData[] PlayerSquadCharacters { get; }
        //public Inventory PlayerInventory { get; }//cant serialize sub-classes
        public StrategyStats StatsUpgrades { get; }
        public IReadOnlyDictionary<GameCurrency, int> Currencies => _currencies;
    }
}
