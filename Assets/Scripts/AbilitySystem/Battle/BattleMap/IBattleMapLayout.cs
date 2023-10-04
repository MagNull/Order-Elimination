using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.Battle
{
    public interface IBattleMapLayout
    {
        public int Width { get; }
        public int Height { get; }

        //Spawn priorities
        //Spawn side
        //Spawns for reinforcements?
        //Spawn class priorities?
        public BattleSpawnData[] GetSpawns();//index is priority
        public StructureSpawnData[] GetStructures();
        public CharacterSpawnData[] GetCharacters();
    }

    public class BattleSpawnData
    {
        public Vector2Int Position { get; set; }
        public EnumMask<BattleSide> SpawningSides { get; set; }
        //public int Priority { get; set; }
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

        public IBattleStructureTemplate StructureTemplate { get; set; }
        public BattleSide Side { get; set; }
        public Vector2Int Position { get; set; }
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

        public IGameCharacterTemplate CharacterTemplate { get; set; }
        public BattleSide Side { get; set; }
        public Vector2Int Position { get; set; }
    }
}
