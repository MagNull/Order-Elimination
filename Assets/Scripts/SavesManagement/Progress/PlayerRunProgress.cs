using GameInventory;
using OrderElimination.MacroGame;
using System;
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
        public Inventory PlayerInventory { get; set; }
        public Guid CurrentPointId { get; set; }
        public Dictionary<Guid, bool> PassedPoints { get; set; } = new();
        public Guid CurrentMap { get; set; }
    }
}
