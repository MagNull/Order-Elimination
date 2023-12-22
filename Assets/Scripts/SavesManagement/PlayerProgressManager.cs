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
        private static int _startMoney = 1000;//Replace with start progress (progress SO template)
        private static IPlayerProgressStorage _progressStorage = new LocalProgressStorage();
        private static SaveDataPacker _saveDataPacker = new SaveDataPacker();
        private static PlayerData _localPlayer = new PlayerData();

        public static bool HasProgress()
        {
            return _progressStorage.ContainsProgressData(_localPlayer);
        }

        public static void LoadPlayerProgress(
            ScenesMediator mediator, IContainerBuilder builder)
        {
            var localPlayer = new PlayerData();
            //Getting values
            var inventory = InventorySerializer.Load();
            var roguelikeMoney = PlayerPrefs.GetInt("Wallet");
            PlayerProgressData progress = null;
            if (_progressStorage.ContainsProgressData(localPlayer))
            {
                _saveDataPacker.RefreshMappings();
                var localData = _progressStorage.GetPlayerProgress(localPlayer);
                progress = _saveDataPacker.UnpackSaveData(localData);
            }

            //Setting values
            if (progress != null)
            {
                if (progress.PlayerCharacters.Length == 0)
                {
                    Logging.LogError(new LocalDataCorruptedException("Squad members count is 0"));
                }
                else
                {
                    mediator.Register("player characters", progress.PlayerCharacters);
                    //localData.PlayerInventory;
                    mediator.Register("stats", progress.StatsUpgrades);
                }
                roguelikeMoney = progress.Currencies[GameCurrency.Roguelike];
            }
            builder.Register<Wallet>(Lifetime.Singleton).WithParameter(roguelikeMoney);
            builder.RegisterComponent(inventory);
            Logging.Log("Player progress data loaded.");
        }

        public static void SavePlayerProgress(
            ScenesMediator mediator, IObjectResolver resolver)
        {
            var localPlayer = new PlayerData();
            _saveDataPacker.RefreshMappings();
            var inventory = resolver.Resolve<Inventory>();
            var playerSquad = mediator.Get<IEnumerable<GameCharacter>>("player characters");
            var upgradeStats = mediator.Get<StrategyStats>("stats");
            var roguelikeMoney = resolver.Resolve<Wallet>().Money;
            var currencies = new Dictionary<GameCurrency, int>()
            {
                { GameCurrency.Roguelike, roguelikeMoney },
            };
            var progress = new PlayerProgressData(
                playerSquad.ToArray(), upgradeStats, currencies);

            PlayerPrefs.SetInt("Wallet", roguelikeMoney);
            InventorySerializer.Save(inventory);
            var save = _saveDataPacker.PackSaveData(progress);
            _progressStorage.SetPlayerProgress(localPlayer, save);

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
