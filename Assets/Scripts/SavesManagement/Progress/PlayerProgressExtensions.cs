using OrderElimination.Infrastructure;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    public static class PlayerProgressExtensions
    {
        public static PlayerMetaProgress GetDefaultMetaProgress()
        {
            var progress = new PlayerMetaProgress()
            {
                StatUpgrades = new(),
                MetaCurrency = 2000,
                StartRunCurrency = 1000,
                HireCurrencyLimit = 1200,
                MaxSquadSize = 3,
                RunInventorySize = 100,
                UnlockedCharacters = AssetIdsMappings.CharactersMapping
                .GetEntries().TakeRandom(5).Select(kv => kv.data).ToList()
            };
            return progress;
        }

        public static PlayerRunProgress GetInitialRunProgress(PlayerMetaProgress metaProgress)
        {
            return new PlayerRunProgress()
            {
                PosessedCharacters = new(),
                PlayerInventory = new(metaProgress.RunInventorySize),
                RunCurrency = metaProgress.StartRunCurrency
            };
        }
    }
}
