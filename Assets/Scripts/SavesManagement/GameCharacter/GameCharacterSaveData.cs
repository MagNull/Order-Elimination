using GameInventory;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct GameCharacterSaveData
    {
        public Guid TemplateId { get; }
        public GameCharacterStats CharacterStats { get; }
        public float CurrentHealth { get; }
        //public Inventory CharacterInventory { get; }

        public GameCharacterSaveData(
            Guid templateId, 
            GameCharacterStats characterStats,
            float currentHealth)//,
            //Inventory inventory)
        {
            TemplateId = templateId;
            CharacterStats = characterStats;
            CurrentHealth = currentHealth;
            //CharacterInventory = inventory;
        }
    }
}
