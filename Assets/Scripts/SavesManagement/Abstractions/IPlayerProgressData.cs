using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.SavesManagement
{
    public interface IPlayerProgressData
    {
        public GameCharacter[] PlayerCharacters { get; }
        //Mapping/Predicate/ids activeSquadMembers,
        //int CurrentPointLocation
        public StrategyStats StatsUpgrades { get; }
        public IReadOnlyDictionary<GameCurrency, int> Currencies { get; }
        //public Inventory PlayerInventory => _playerInventory;
    }
}
