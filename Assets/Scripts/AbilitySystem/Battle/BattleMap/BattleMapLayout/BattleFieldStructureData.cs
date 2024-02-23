using OrderElimination.Infrastructure;
using UnityEngine;

namespace OrderElimination.Battle
{
    public class BattleFieldStructureData
    {
        public BattleFieldStructureData(
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
}
