using GameInventory;
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
        private readonly GameCharacterSaveData[] _playerCharacters;
        private readonly StrategyStats _statsUpgrades;
        //private readonly Inventory _playerInventory;

        public PlayerProgressSerializableData(
            GameCharacterSaveData[] playerCharacters,
            StrategyStats statsUpgrades,
            IReadOnlyDictionary<GameCurrency, int> currencies)
        {
            _playerCharacters = playerCharacters;
            _statsUpgrades = statsUpgrades;
            _currencies = currencies.ToDictionary(kv => kv.Key, kv => kv.Value);
            //_playerInventory = playerInventory;
        }

        public GameCharacterSaveData[] PlayerCharacters => _playerCharacters;
        //public Inventory PlayerInventory => _playerInventory;
        public StrategyStats StatsUpgrades => _statsUpgrades;
        public IReadOnlyDictionary<GameCurrency, int> Currencies => _currencies;
    }
}
