using OrderElimination.MacroGame;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    public class PlayerProgressData
    {
        private readonly Dictionary<GameCurrency, int> _currencies;

        public PlayerProgressData(
            GameCharacter[] playerCharacters, 
            StrategyStats statsUpgrades, 
            IReadOnlyDictionary<GameCurrency, int> currencies)
        {
            PlayerCharacters = playerCharacters;
            StatsUpgrades = statsUpgrades;
            _currencies = currencies.ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public GameCharacter[] PlayerCharacters { get; }
        //Mapping/Predicate/ids activeSquadMembers,
        //Inventory playerInventory,
        //int CurrentPointLocation
        public StrategyStats StatsUpgrades { get; }
        public IReadOnlyDictionary<GameCurrency, int> Currencies => _currencies;
        
    }
}
