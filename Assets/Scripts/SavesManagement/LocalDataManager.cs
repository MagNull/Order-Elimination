using GameInventory;
using OrderElimination.GameContent;
using OrderElimination.Infrastructure;
using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace OrderElimination.SavesManagement
{
    public static class LocalDataManager
    {
        public static IFormatter Formatter { get; private set; } = new BinaryFormatter();
        public static string LocalDataFileExtension => ".oesave";
        public static string LocalProgressSavePath => Path.Combine(Application.persistentDataPath, "Saves");
        public static bool IsLocalDataExists
        {
            get
            {
                if (!Directory.Exists(LocalProgressSavePath))
                {
                    return false;
                }
                if (Directory.EnumerateFiles(LocalProgressSavePath, $"*{LocalDataFileExtension}").Any())
                {
                    //Check for corruption
                    return true;
                }
                return false;
            }
        }

        public static PlayerProgressSaveData LoadLatestLocalData()
        {
            if (!Directory.Exists(LocalProgressSavePath))
                throw new FailedToLoadLocalDataException(
                    $"Local data directory wasn't found at {LocalProgressSavePath}.");
            var localDatas = Directory
                .GetFiles(LocalProgressSavePath, $"*{LocalDataFileExtension}")
                .Select(name => new FileInfo(name))
                .OrderByDescending(f => f.CreationTimeUtc)
                .ToArray();
            if (localDatas.Length == 0)
                throw new FailedToLoadLocalDataException(
                    $"Local data file wasn't found at {LocalProgressSavePath}.");
            FileStream fileStream;
            fileStream = new FileStream(localDatas[0].FullName, FileMode.Open);//latest
            var deserializedObject = Formatter.Deserialize(fileStream);
            if (deserializedObject is not PlayerProgressSaveData playerProgress)
                throw new FailedToLoadLocalDataException("Local data file is corrupted.");//try to get another
            var latestLocalData = (PlayerProgressSaveData)deserializedObject;
            return latestLocalData;
        }

        public static void WriteLocalData(PlayerProgressSaveData saveData)
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
            var fileStream = new FileStream($"{fileName}({id}){LocalDataFileExtension}", FileMode.CreateNew);
            Formatter.Serialize(fileStream, saveData);
            fileStream.Close();
        }

        private static string GetDatedFileNameWithoutExtension(string path, DateTime creationTimeUtc)
        {
            var shortName =
                $"LocalProgress-{creationTimeUtc.Year}-{creationTimeUtc.Month}-{creationTimeUtc.Day}";
            return Path.Combine(path, shortName);
        }
    }
}
