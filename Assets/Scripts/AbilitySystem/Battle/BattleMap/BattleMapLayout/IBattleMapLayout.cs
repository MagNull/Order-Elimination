using OrderElimination.Infrastructure;
using UnityEngine;

namespace OrderElimination.Battle
{
    public interface IBattleMapLayout
    {
        public string MapName { get; }
        public int Width { get; }
        public int Height { get; }

        //Spawn priorities
        //Spawn side
        //Spawns for reinforcements?
        //Spawn class priorities?
        public BattleSpawnData[] GetSpawns();//index = priority
        public StructureSpawnData[] GetStructures();
        public CharacterSpawnData[] GetCharacters();
    }

    public class BattleSpawnData
    {
        public Vector2Int Position { get; }

        public EnumMask<BattleSide> SpawningSides { get; } = new();
        //public bool IsReinforcementSpawn { get; set; } = false;
        //public int Priority { get; set; } //dictated by index in list

        public BattleSpawnData(Vector2Int position, EnumMask<BattleSide> spawningSides)
        {
            Position = position;
            SpawningSides = spawningSides.Clone();
        }
    }

    public class StructureSpawnData
    {
        public StructureSpawnData(
            IBattleStructureTemplate structureTemplate, BattleSide side, Vector2Int position)
        {
            StructureTemplate = structureTemplate;
            Side = side;
            Position = position;
        }

        public IBattleStructureTemplate StructureTemplate { get; }
        public BattleSide Side { get; }
        public Vector2Int Position { get; }
    }

    public class CharacterSpawnData
    {
        public CharacterSpawnData(
            IGameCharacterTemplate characterTemplate, BattleSide side, Vector2Int position)
        {
            CharacterTemplate = characterTemplate;
            Side = side;
            Position = position;
        }

        public IGameCharacterTemplate CharacterTemplate { get; }
        public BattleSide Side { get; }
        public Vector2Int Position { get; }
    }
}
