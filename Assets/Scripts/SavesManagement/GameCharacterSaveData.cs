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
        public readonly string BasedTemplateLocation;
        public readonly GameCharacterStats CharacterStats;

        public GameCharacterSaveData(string basedTemplateLocation, GameCharacterStats characterStats)
        {
            BasedTemplateLocation = basedTemplateLocation;
            CharacterStats = characterStats;
        }
    }
}
