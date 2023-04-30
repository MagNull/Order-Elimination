using System;
using System.Collections.Generic;
using Inventory_Items;

namespace RoguelikeMap.Panels
{
    public class SafeZonePanel : Panel
    {
        public event Action OnHealAccept;
        public event Action<IReadOnlyList<ItemData>> OnLootAccept;
        
        public void HealAccept()
        {
            OnHealAccept?.Invoke();
            Close();
        }

        //TODO(coder): add loot to player inventory after create inventory system
        public void LootAccept()
        {
            OnLootAccept?.Invoke(null);
            Close();
        }
    }
}