using GameInventory;
using GameInventory.Items;
using OrderElimination.MacroGame;
using System.Collections.Generic;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    public class PlayerProgressData : IPlayerProgressData
    {
        //--- MetaGame ---
        //(*const per real player)
        public int MetaCurrency { get; set; }
        public int HireCurrencyLimit { get; set; } //(used to hire characters for Roguelike Run)
        //public HashSet<CharacterTemplate> UnlockedCharacters 
        //public HashSet<ItemData> KnownItems

        //--- Roguelike Run --- 
        //(*resets each "new game")
        public StrategyStats StatUpgrades { get; set; }//NEW GAME RESETS
        // Modifiers
        // Map (points, locations, enemies)

        public int RoguelikeCurrency { get; set; }//NEW GAME RESETS
        public GameCharacter[] PosessedCharacters { get; set; }//NEW GAME RESETS
        // Characters metadata ? (id, isActiveInSquad, isHired, ...)
        // Inventory
        // Current point



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
