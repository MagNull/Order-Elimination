using OrderElimination.Infrastructure;
using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public class LocalProgressStorage : IPlayerProgressStorage
    {
        private IFileSerializer _serializer = new BinaryFileSerializer();

        public static string LocalDataFileExtension => ".oesave";
        public static string LocalProgressSavePath => $"{Application.persistentDataPath}/Saves";

        public bool ContainsProgressData(PlayerData player)
        {
            if (!Directory.Exists(LocalProgressSavePath))
            {
                return false;
            }
            return Directory.EnumerateFiles(LocalProgressSavePath, $"*{LocalDataFileExtension}")
                .Any(f => IsSaveFileValid(new FileInfo(f)));
        }

        public PlayerProgressSerializableData GetPlayerProgress(PlayerData player)
        {
            if (!ContainsProgressData(player))
                throw new FailedToLoadLocalDataException(
                    $"Local data wasn't found at {LocalProgressSavePath}.");
            var localDatas = Directory
                .GetFiles(LocalProgressSavePath, $"*{LocalDataFileExtension}")
                .Select(name => new FileInfo(name))
                .Where(f => IsSaveFileValid(f))
                .OrderByDescending(f => f.CreationTimeUtc)
                .ToArray();
            var deserializedObject = _serializer.Deserialize(localDatas.First().FullName);
            return (PlayerProgressSerializableData)deserializedObject;
        }

        public void SetPlayerProgress(PlayerData player, PlayerProgressSerializableData saveData)
        {
            if (!Directory.Exists(LocalProgressSavePath))
            {
                Directory.CreateDirectory(LocalProgressSavePath);
            }
            var fileName = GetDatedFileNameWithoutExtension(LocalProgressSavePath, DateTime.Now);
            var id = 0;
            while (File.Exists($"{fileName}({id}){LocalDataFileExtension}"))
            {
                id++;
            }
            _serializer.Serialize($"{fileName}({id}){LocalDataFileExtension}", saveData);
        }

        private static string GetDatedFileNameWithoutExtension(string path, DateTime creationTimeUtc)
        {
            var shortName =
                $"LocalProgress-{creationTimeUtc.Year}-{creationTimeUtc.Month}-{creationTimeUtc.Day}";
            return Path.Combine(path, shortName);
        }

        private bool IsSaveFileValid(FileInfo file)
        {
            object deserializedObject;
            try
            {
                deserializedObject = _serializer.Deserialize(file.FullName);
            }
            catch
            {
                return false;
            }
            if (deserializedObject == null
                || deserializedObject is not PlayerProgressSerializableData progress)
                return false;
            //Check values
            return true;
        }
    }
}
