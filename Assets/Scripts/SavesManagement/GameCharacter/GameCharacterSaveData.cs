using GameInventory;
using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct GameCharacterSaveData
    {
        public Guid CharacterTemplateId { get; }
        public GameCharacterStats CharacterStats { get; }
        public float CurrentHealth { get; }
        //public Inventory CharacterInventory { get; }

        public GameCharacterSaveData(
            Guid templateId, 
            GameCharacterStats characterStats,
            float currentHealth)//,
            //Inventory inventory)
        {
            CharacterTemplateId = templateId;
            CharacterStats = characterStats;
            CurrentHealth = currentHealth;
            //CharacterInventory = inventory;
        }
    }
}
