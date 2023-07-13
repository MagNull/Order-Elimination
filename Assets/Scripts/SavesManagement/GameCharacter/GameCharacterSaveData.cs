using GameInventory;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct GameCharacterSaveData
    {
        public int BasedTemplateId { get; }
        public GameCharacterStats CharacterStats { get; }
        public float CurrentHealth { get; }
        public Inventory CharacterInventory { get; }

        public GameCharacterSaveData(
            int basedTemplateId, 
            GameCharacterStats characterStats,
            float currentHealth,
            Inventory inventory)
        {
            BasedTemplateId = basedTemplateId;
            CharacterStats = characterStats;
            CurrentHealth = currentHealth;
            CharacterInventory = inventory;
        }
    }
}
