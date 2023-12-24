using GameInventory;
using OrderElimination.MacroGame;
using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public static class PlayerProgressManager
    {
        private static IPlayerProgressStorage _progressStorage = new LocalProgressStorage();
        private static SaveDataPacker _saveDataPacker = new SaveDataPacker();
        private static PlayerData _localPlayer = new PlayerData();

        public static IPlayerProgressData NewGameProgress { get; } = new PlayerProgressData(
            new GameCharacter[0],
            new StrategyStats(),
            new Dictionary<GameCurrency, int>()
            {
                { GameCurrency.Roguelike, 1800 }
            });

        public static bool HasProgress()
        {
            var hasProgress = _progressStorage.ContainsProgressData(_localPlayer);
            Logging.Log($"Has local progress: {hasProgress}");
            return hasProgress;
        }

        public static bool LoadPlayerProgress(
            out IPlayerProgressData progress, out Inventory playerInventory)
        {
            _saveDataPacker.RefreshMappings();
            //Getting values
            playerInventory = InventorySerializer.Load();
            var roguelikeMoney = UnityEngine.PlayerPrefs.GetInt("Wallet");
            progress = null;
            try
            {
                if (_progressStorage.ContainsProgressData(_localPlayer))
                {
                    var localData = _progressStorage.GetPlayerProgress(_localPlayer);
                    progress = _saveDataPacker.UnpackSaveData(localData);
                }
                else
                    return false;
            }
            catch (System.Exception e)
            {
                Logging.LogException(e);
            }

            //Setting values
            if (progress != null)
            {
                if (progress.PlayerCharacters.Length > 0)
                {
                    return true;
                }
                else
                {
                    Logging.LogError(new LocalDataCorruptedException("Squad members count is 0"));
                }
            }
            return false;
        }

        public static void SavePlayerProgress(
            IPlayerProgressData progress, Inventory playerInventory)
        {
            _saveDataPacker.RefreshMappings();
            InventorySerializer.Save(playerInventory);
            var save = _saveDataPacker.PackSaveData(progress);
            _progressStorage.SetPlayerProgress(_localPlayer, save);
        }

        public static void ClearPlayerProgress()
        {
            //Remove Local Data
            _progressStorage.ClearPlayerProgress(_localPlayer);
            InventorySerializer.Delete();
            Logging.Log("Player progress data cleared.");
        }
    }
}
