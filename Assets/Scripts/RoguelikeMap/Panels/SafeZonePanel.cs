using System;
using DG.Tweening;

namespace RoguelikeMap.Panels
{
    public class SafeZonePanel : IPanel
    {
        public event Action OnHealAccept;
        public event Action OnLootAccept;
        
        public void HealAccept()
        {
            OnHealAccept?.Invoke();
            Close();
        }

        //TODO(coder): add loot to player inventory after create inventory system
        public void LootAccept()
        {
            OnLootAccept?.Invoke();
            Close();
        }
    }
}