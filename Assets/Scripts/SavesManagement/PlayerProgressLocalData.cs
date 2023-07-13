using GameInventory;
using System;

namespace OrderElimination.SavesManagement
{
    [Serializable]
    public readonly struct PlayerProgressLocalData
    {
        public GameCharacterSaveData[] PlayerSquadCharacters { get; }
        //public Inventory PlayerInventory { get; }//cant serialize sub-classes
        public StrategyStats StatsUpgrades { get; }

        public static PlayerProgressLocalData Empty => new (
            new GameCharacterSaveData[0], new StrategyStats());


        public PlayerProgressLocalData(
            GameCharacterSaveData[] playerSquadCharacters,
            StrategyStats statsUpgrades)
        {
            PlayerSquadCharacters = playerSquadCharacters;
            StatsUpgrades = statsUpgrades;
        }
    }
}
