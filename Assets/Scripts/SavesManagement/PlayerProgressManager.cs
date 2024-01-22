using GameInventory;
using System;

namespace OrderElimination.SavesManagement
{
    public static class PlayerProgressManager
    {
        private static readonly IPlayerProgressStorage _progressStorage;
        private static readonly SaveDataPacker _saveDataPacker;
        private static PlayerData _localPlayer = new PlayerData();
        private static IPlayerProgress _lastLoadedProgress;

        static PlayerProgressManager()
        {
            AssetIdsMappings.RefreshMappings();
            _saveDataPacker = new(AssetIdsMappings.CharactersMapping);
            _progressStorage = new LocalProgressStorage(_saveDataPacker);
        }

        public static IPlayerProgress LoadSavedProgress()
        {
            //ToRemove
            var playerInventory = InventorySerializer.Load();
            var roguelikeMoney = UnityEngine.PlayerPrefs.GetInt("Wallet");
            //

            var localProgress = _progressStorage.GetProgress(_localPlayer);
            if (localProgress != null)
            {
                //Validate
                if (!ProgressIsValid(localProgress))
                    Logging.LogError(new LocalDataCorruptedException());
                _lastLoadedProgress = localProgress;
                return localProgress;
            }

            //return default progress
            return new PlayerProgress()
            {
                CurrentRunProgress = null
            };
            throw new NotImplementedException();
        }

        public static IPlayerProgress GetLastLoadedProgress()
            => _lastLoadedProgress;

        public static void SaveProgress(IPlayerProgress progress)
        {
            if (progress == null)
                throw new ArgumentNullException();
            _progressStorage.SetProgress(_localPlayer, progress);
            //InventorySerializer.Save(playerInventory);
        }

        public static void ClearProgress()
        {
            //Remove Local Data
            _progressStorage.ClearProgress(_localPlayer);
            InventorySerializer.Delete();
            Logging.Log("Player progress data cleared.");
        }

        private static bool ProgressIsValid(IPlayerProgress progress)
        {
            if (progress.CurrentRunProgress != null)
            {
                if (progress.CurrentRunProgress.PosessedCharacters.Length == 0)
                    return false;
            }
            return true;
        }
    }
}
