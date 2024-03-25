using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.Battle
{
    public class BattleMapLayoutSpawn
    {
        [ShowInInspector, OdinSerialize]
        public float PriorityOrder { get; set; }

        [ShowInInspector, OdinSerialize]
        public EnumMask<BattleSide> SpawningSides { get; set; } = EnumMask<BattleSide>.Empty;

        public BattleMapLayoutSpawn() { }

        public BattleMapLayoutSpawn(EnumMask<BattleSide> spawningSides)
        {
            SpawningSides = spawningSides;
        }
    }
}
