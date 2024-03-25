using OrderElimination.Infrastructure;
using UnityEngine;

namespace OrderElimination.Battle
{
    public class BattleFieldCharacterData
    {
        public BattleFieldCharacterData(
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
