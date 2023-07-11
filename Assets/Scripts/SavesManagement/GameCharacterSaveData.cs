using OrderElimination.MacroGame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct GameCharacterSaveData
    {
        public string BasedTemplateLocation { get; }
        public GameCharacterStats CharacterStats { get; }
        public float CurrentHealth { get; }

        public GameCharacterSaveData(
            string basedTemplateLocation, 
            GameCharacterStats characterStats,
            float currentHealth)
        {
            BasedTemplateLocation = basedTemplateLocation;
            CharacterStats = characterStats;
            CurrentHealth = currentHealth;
        }
    }
}
