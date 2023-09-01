using OrderElimination.Infrastructure;
using System;

namespace OrderElimination.AbilitySystem
{
    public interface ITurnPriority
    {
        public BattleSide GetNextTurnSide(BattleSide currentTurnSide);
        public BattleSide GetStartingSide();
    }

    public class PlayerFirstTurnPriority : ITurnPriority
    {
        public BattleSide GetNextTurnSide(BattleSide currentTurnSide)
        {
            return currentTurnSide switch
            {
                BattleSide.NoSide => BattleSide.Player,
                BattleSide.Player => BattleSide.Allies,
                BattleSide.Allies => BattleSide.Enemies,
                BattleSide.Enemies => BattleSide.Others,
                BattleSide.Others => BattleSide.NoSide,
                _ => throw new NotSupportedException(),
            };
        }

        public BattleSide GetStartingSide() => BattleSide.Player;
    }
}
