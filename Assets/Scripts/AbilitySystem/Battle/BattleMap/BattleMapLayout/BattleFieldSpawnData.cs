using OrderElimination.Infrastructure;
using UnityEngine;

namespace OrderElimination.Battle
{
    public class BattleFieldSpawnData
    {
        public Vector2Int Position { get; }

        public EnumMask<BattleSide> SpawningSides { get; } = new();
        //public bool IsReinforcementSpawn { get; set; } = false;
        //public int Priority { get; set; } //dictated by index in list
        //Spawn class priorities?

        public BattleFieldSpawnData(Vector2Int position, EnumMask<BattleSide> spawningSides)
        {
            Position = position;
            SpawningSides = spawningSides.Clone();
        }
    }
}
