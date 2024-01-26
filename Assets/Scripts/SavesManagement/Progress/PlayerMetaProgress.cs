using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public class PlayerMetaProgress
    {
        public UpgradeStats StatUpgrades { get; set; }
        public int MetaCurrency { get; set; }
        public int StartRunCurrency { get; set; }
        public int HireCurrencyLimit { get; set; } //(used to hire characters for Roguelike Run)
        //int RunInventorySize
        //public ISet<CharacterTemplate> UnlockedCharacters { get; set; }
        //public HashSet<ItemData> KnownItems
    }
}
