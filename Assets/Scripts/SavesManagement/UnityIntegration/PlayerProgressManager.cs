using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Linq;

namespace OrderElimination.SavesManagement
{
    public class PlayerProgressManager : SerializedMonoBehaviour, IPlayerProgressManager
    {
        [OdinSerialize]
        private IPlayerProgress _defaultProgress;

        private IPlayerProgress _lastLoadedProgress;

        private IPlayerProgressStorage _progressStorage;
        private SaveDataPacker _saveDataPacker;

        private PlayerData _localPlayer = new PlayerData();

        private void Awake()
        {
            DontDestroyOnLoad(this);
            //TODO-SAVES: load all assets
            AssetIdsMappings.RefreshMappings();
            _saveDataPacker = new(
                AssetIdsMappings.CharactersMapping,
                AssetIdsMappings.ItemsMapping);
            _progressStorage = new LocalProgressStorage(_saveDataPacker);
            if (_defaultProgress != null)
            {
                var copy = _saveDataPacker.UnpackSaveData(
                    _saveDataPacker.PackSaveData(_defaultProgress));
                _defaultProgress = copy;
            }
            else
            {
                _defaultProgress = new PlayerProgress()
                {
                    MetaProgress = PlayerProgressExtensions.GetDefaultMetaProgress()
                };
            }
        }

        public IPlayerProgress GetPlayerProgress()
        {
            if (_lastLoadedProgress != null)
                return _lastLoadedProgress;
            var savedProgress = LoadSavedProgress();
            if (savedProgress == null)
            {
                Logging.LogError("Saved progress not found");
                return AssignNewProgress(_defaultProgress);
            }
            if (!IsProgressValid(savedProgress))
            {
                Logging.LogError("Saved progress is corrupted");
                return AssignNewProgress(_defaultProgress);
            }
            return AssignNewProgress(savedProgress);
        }

        private IPlayerProgress LoadSavedProgress()
        {
            return _progressStorage.GetProgress(_localPlayer);
        }

        public void SaveProgress()
        {
            var progress = _lastLoadedProgress ?? _defaultProgress;
            if (progress == null)
                throw new ArgumentNullException();
            _progressStorage.SetProgress(_localPlayer, progress);
        }

        //[DisableInPlayMode]
        //[Button]
        public void ClearProgress()
        {
            _progressStorage.ClearProgress(_localPlayer);
            //TODO-SAVE: will not affect already utilized progress
            //_lastLoadedProgress = _defaultProgress;
            Logging.Log("Player progress data cleared.");
        }

        private IPlayerProgress AssignNewProgress(IPlayerProgress progress)
        {
            _lastLoadedProgress = progress;
            return _lastLoadedProgress;
        }

        private bool IsProgressValid(IPlayerProgress progress)
        {
            if (progress.CurrentRunProgress != null)
            {
                if (progress.CurrentRunProgress.PosessedCharacters.Count == 0)
                    return false;
            }
            return true;
        }

        public void OnDisable()
        {
            SaveProgress();
        }
    }
}
