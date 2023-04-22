using System;
using System.Collections.Generic;
using DG.Tweening;

namespace RoguelikeMap.Panels
{
    public class SafeZonePanel : Panel
    {
        public event Action OnHealAccept;
        public event Action<IReadOnlyList<int>> OnLootAccept;
        
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