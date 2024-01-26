using System;
using System.IO;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public class LocalProgressStorage : IPlayerProgressStorage
    {
        public static string SaveFileExtension => ".oesave";
        public static string LocalSavesPath => $"{Application.persistentDataPath}/Saves";

        private readonly SaveDataPacker _saveDataPacker;

        public LocalProgressStorage(SaveDataPacker saveDataPacker)
        {
            AssetIdsMappings.RefreshMappings();
            _saveDataPacker = saveDataPacker;
        }

        public IPlayerProgress GetProgress(PlayerData player)
        {
            var path = LocalSavesPath;
            if (!Directory.Exists(path))
                return null;
            var targetFile = GetSaveFileFullName(player);
            if (!File.Exists(targetFile))
                return null;
            if (!IsSaveFileValid(new FileInfo(targetFile), out var progress))
                return null;
            return progress;
        }

        public bool SetProgress(PlayerData player, IPlayerProgress progress)
        {
            if (progress == null)
                throw new ArgumentNullException("Progress to save can not be null");
            var path = LocalSavesPath;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(LocalSavesPath);
            var targetFile = GetSaveFileFullName(player);
            if (File.Exists(targetFile))
            {
                var backupFile = GetBackupFileFullName(player);
                File.Delete(backupFile);
                File.Copy(targetFile, backupFile);
            }
            var json = _saveDataPacker.PackSaveData(progress);
            File.WriteAllText(targetFile, json);
            Logging.Log($"Progress saved at \"{targetFile}\"");
            return true;
        }

        public void ClearProgress(PlayerData player)
        {
            var targetFile = GetBackupFileFullName(player);
            if (File.Exists(targetFile))
                File.Delete(targetFile);
        }

        private bool IsSaveFileValid(FileInfo file, out IPlayerProgress progress)
        {
            try
            {
                var text = File.ReadAllText(file.FullName);
                var deserializedObject = _saveDataPacker.UnpackSaveData(text);
                progress = deserializedObject;
            }
            catch (Exception e)
            {
                Logging.LogError("Error occured while attempting to load a saved progress");
                Logging.LogError(e);
                progress = null;
                return false;
            }
            return true;
        }

        private string GetSaveFileName(PlayerData player)
            => $"PlayerProgress-Default" + SaveFileExtension;

        private string GetSaveFileFullName(PlayerData player)
            => $"{LocalSavesPath}/{GetSaveFileName(player)}";

        private string GetBackupFileName(PlayerData player)
        {
            var now = DateTime.Now;
            return $"bc-{now.Month}-{now.Day}-{GetSaveFileName(player)}";
        }

        private string GetBackupFileFullName(PlayerData player)
            => $"{LocalSavesPath}/{GetBackupFileName(player)}";
    }
}
