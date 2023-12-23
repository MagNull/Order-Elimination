using GameInventory;
using Newtonsoft.Json;
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
            var hasProgress = _progressStorage.ContainsProgressData(_localPlayer);
            Logging.Log($"Has local progress: {hasProgress}");
            return hasProgress;
        }

        public static void LoadPlayerProgress(
            ScenesMediator mediator, IContainerBuilder builder)
        {
            _saveDataPacker.RefreshMappings();
            //Getting values
            var inventory = InventorySerializer.Load();
            var roguelikeMoney = PlayerPrefs.GetInt("Wallet");
            PlayerProgressData progress = null;
            try
            {
                if (_progressStorage.ContainsProgressData(_localPlayer))
                {
                    var localData = _progressStorage.GetPlayerProgress(_localPlayer);
                    progress = _saveDataPacker.UnpackSaveData(localData);
                }
            }
            catch (System.Exception e)
            {
                Logging.LogException(e);
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
            _progressStorage.SetPlayerProgress(_localPlayer, save);

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
