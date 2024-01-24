using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public class PlayerMetaProgress
    {
        public StrategyStats StatUpgrades { get; set; }
        public int MetaCurrency { get; set; }
        public int HireCurrencyLimit { get; set; } //(used to hire characters for Roguelike Run)
        //public ISet<CharacterTemplate> UnlockedCharacters { get; set; }
        //public HashSet<ItemData> KnownItems
    }
}
