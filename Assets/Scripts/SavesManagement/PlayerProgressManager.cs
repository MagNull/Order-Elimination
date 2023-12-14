using GameInventory;
using OrderElimination.MacroGame;
using StartSessionMenu;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace OrderElimination.SavesManagement
{
    public static class PlayerProgressManager
    {
        private static int _startMoney = 1000;

        public static void LoadPlayerProgress(
            ScenesMediator mediator, IContainerBuilder builder)
        {
            SaveDataPacker.RefreshMappings();
            var inventory = InventorySerializer.Load();
            var money = PlayerPrefs.GetInt("Wallet");

            builder.Register<Wallet>(Lifetime.Singleton).WithParameter(money);
            builder.RegisterComponent(inventory);

            if (LocalDataManager.IsLocalDataExists)
            {
                var localData = LocalDataManager.LoadLatestLocalData();
                var playerCharacters = SaveDataPacker.UnpackPlayerCharacters(localData);
                if (playerCharacters.Length == 0)
                {
                    Logging.LogError(new LocalDataCorruptedException("Squad members count is 0"));
                }
                else
                {
                    mediator.Register("player characters", playerCharacters);
                    //localData.PlayerInventory;
                    mediator.Register("stats", localData.StatsUpgrades);
                }
            }
            Logging.Log("Player progress data loaded.");
        }

        public static void SavePlayerProgress(
            ScenesMediator mediator, IObjectResolver resolver)
        {
            SaveDataPacker.RefreshMappings();
            var inventory = resolver.Resolve<Inventory>();
            var playerSquad = mediator.Get<IEnumerable<GameCharacter>>("player characters");
            var upgradeStats = mediator.Get<StrategyStats>("stats");

            PlayerPrefs.SetInt("Wallet", resolver.Resolve<Wallet>().Money);
            InventorySerializer.Save(inventory);
            var save = SaveDataPacker.PackSaveData(playerSquad.ToArray(), upgradeStats);
            LocalDataManager.WriteLocalData(save);

            Logging.Log("Player progress data saved.");
        }

        public static void ClearPlayerProgress()
        {
            InventorySerializer.Delete();
            PlayerPrefs.SetInt("Wallet", _startMoney);
            //Remove Local Data
            Logging.Log("Player progress data cleared.");
        }
    }
}
