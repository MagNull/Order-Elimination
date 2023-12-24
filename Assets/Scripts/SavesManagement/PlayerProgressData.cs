using GameInventory;
using OrderElimination.MacroGame;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    public class PlayerProgressData : IPlayerProgressData
    {
        private readonly Dictionary<GameCurrency, int> _currencies;
        private GameCharacter[] _playerCharacters;
        private StrategyStats _statsUpgrades;
        //private Inventory _playerInventory;

        public PlayerProgressData(
            GameCharacter[] playerCharacters, 
            StrategyStats statsUpgrades, 
            IReadOnlyDictionary<GameCurrency, int> currencies)
        {
            _playerCharacters = playerCharacters;
            _statsUpgrades = statsUpgrades;
            //_playerInventory = playerInventory;
            _currencies = currencies.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public GameCharacter[] PlayerCharacters => _playerCharacters;
        //Mapping/Predicate/ids activeSquadMembers,
        //int CurrentPointLocation
        public StrategyStats StatsUpgrades => _statsUpgrades;
        public IReadOnlyDictionary<GameCurrency, int> Currencies => _currencies;
        //public Inventory PlayerInventory => _playerInventory;

    }
}
