using GameInventory;
using OrderElimination.MacroGame;
using System.Collections.Generic;

namespace OrderElimination.SavesManagement
{
    public class PlayerRunProgress
    {
        // - - - Static (*resets each "new game")
        // Modifiers
        // Map (points, locations, enemies)

        // - - - Dynamic (can change during run)
        public int RunCurrency { get; set; }

        //1.Replace with SquadCharacter wrapper
        //2.Characters metadata ? (id, isActiveInSquad, isHired, ...)
        //3.List ActiveCharactersIds
        public List<GameCharacter> PosessedCharacters { get; set; }
        public Inventory PlayerInventory { get; set; }//not used yet
        public int CurrentPointId { get; set; }//not used yet
    }
}
