using OrderElimination.MacroGame;
using System;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct GameCharacterSaveData
    {
        public int BasedTemplate { get; }
        public GameCharacterStats CharacterStats { get; }
        public float CurrentHealth { get; }

        public GameCharacterSaveData(
            int basedTemplateId, 
            GameCharacterStats characterStats,
            float currentHealth)
        {
            BasedTemplate = basedTemplateId;
            CharacterStats = characterStats;
            CurrentHealth = currentHealth;
        }
    }
}
