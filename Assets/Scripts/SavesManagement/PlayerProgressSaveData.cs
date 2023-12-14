using GameInventory;
using System;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct PlayerProgressSaveData
    {
        public GameCharacterSaveData[] PlayerSquadCharacters { get; }
        //public Inventory PlayerInventory { get; }//cant serialize sub-classes
        public StrategyStats StatsUpgrades { get; }

        public static PlayerProgressSaveData Empty => new (
            new GameCharacterSaveData[0], new StrategyStats());


        public PlayerProgressSaveData(
            GameCharacterSaveData[] playerSquadCharacters,
            StrategyStats statsUpgrades)
        {
            PlayerSquadCharacters = playerSquadCharacters;
            StatsUpgrades = statsUpgrades;
        }
    }
}
