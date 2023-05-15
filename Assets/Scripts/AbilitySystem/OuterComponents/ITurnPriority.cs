using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                BattleSide.Player => BattleSide.Allies,
                BattleSide.Enemies => BattleSide.Others,
                BattleSide.Allies => BattleSide.Enemies,
                BattleSide.Others => BattleSide.Player,
                _ => throw new NotSupportedException(),
            };
        }

        public BattleSide GetStartingSide() => BattleSide.Player;
    }
}
