using OrderElimination.MacroGame;

namespace OrderElimination.SavesManagement
{
    public class PlayerRunProgress
    {
        // - - - Static (*resets each "new game")

        public StrategyStats StatUpgrades { get; set; }
        // Modifiers
        // Map (points, locations, enemies)

        // - - - Dynamic (can change during run)

        public int RoguelikeCurrency { get; set; }
        //1.Replace with SquadCharacter wrapper

        //2.Characters metadata ? (id, isActiveInSquad, isHired, ...)
        public GameCharacter[] PosessedCharacters { get; set; }
        // Inventory
        // Current point
    }
}
