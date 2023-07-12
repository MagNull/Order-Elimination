using System.IO;
using UnityEngine;

namespace GameInventory
{
    public static class InventorySerializer
    {
        //Serialize and save inventory to JSON
        public static void Save(Inventory inventory)
        {
            var path = Application.persistentDataPath + "/inventory.json";
            var json = JsonUtility.ToJson(inventory, true);
            File.WriteAllText(path, json);
        }

        public static Inventory Load()
        {
            var path = Application.persistentDataPath + "/inventory.json";
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                if (json == "" || json == "{}s")
                    return new Inventory(100);
                var inventory = JsonUtility.FromJson<Inventory>(json);
                inventory.InitConsumables();
                return inventory;
            }

            return new Inventory(100);
        }
    }
}