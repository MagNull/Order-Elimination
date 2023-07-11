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
            Debug.Log(path);
            var json = JsonUtility.ToJson(inventory, true);
            File.WriteAllText(path, json);
            Debug.Log(json);
            Debug.Log(File.ReadAllText(path));
        }

        public static Inventory Load()
        {
            var path = Application.persistentDataPath + "/inventory.json";
            if (File.Exists(path))
            {
                var json = File.ReadAllText(path);
                return JsonUtility.FromJson<Inventory>(json);
            }

            return new Inventory(100);
        }
    }
}