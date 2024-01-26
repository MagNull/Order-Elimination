using Sirenix.Serialization;
using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public class PlayerMetaProgress
    {
        [OdinSerialize]
        public UpgradeStats StatUpgrades { get; set; } = new();

        [OdinSerialize]
        public int MetaCurrency { get; set; }

        [OdinSerialize]
        public int StartRunCurrency { get; set; }

        [OdinSerialize]
        public int HireCurrencyLimit { get; set; } //used to hire characters for Roguelike Run

        [OdinSerialize]
        public int MaxSquadSize { get; set; }

        [OdinSerialize]
        public List<IGameCharacterTemplate> UnlockedCharacters { get; set; } = new();
        //public int RunInventorySize { get; set; }
        //int MaxMembersToBuyAtTheStart
        //public HashSet<ItemData> KnownItems
    }
}
