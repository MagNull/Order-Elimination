﻿using System.IO;
using UnityEngine;

namespace GameInventory
{
    public static class InventorySerializer
    {
        private static readonly string _path = Application.persistentDataPath + "/inventory.json";
        
        public static void Save(Inventory inventory)
        {
            var json = JsonUtility.ToJson(inventory, true);
            File.WriteAllText(_path, json);
        }

        public static Inventory Load()
        {
            ItemIdentifier.RecoverID();
            if (File.Exists(_path))
            {
                var json = File.ReadAllText(_path);
                if (json == "" || json == "{}")
                    return new Inventory(100);
                var inventory = JsonUtility.FromJson<Inventory>(json);
                inventory.InitConsumables();
                return inventory;
            }

            return new Inventory(100);
        }

        public static void Delete()
        {
            File.Delete(_path);
        }
    }
}