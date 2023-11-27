using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

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

    public class ConfigurableTurnPriority : ITurnPriority
    {
        private bool ContainsAllSides
            => EnumExtensions.GetValues<BattleSide>().All(s => _turnSequenceLoop.Contains(s));

        private bool NoRepeatingElements
        {
            get
            {
                var includedSides = new HashSet<BattleSide>();
                foreach (var side in _turnSequenceLoop)
                {
                    if (includedSides.Contains(side))
                        return false;
                    includedSides.Add(side);
                }
                return true;
            }
        }

        [ValidateInput("@" + nameof(ContainsAllSides), "Sequence must contain all sides!")]
        [ValidateInput("@" + nameof(NoRepeatingElements), "Sides are repeating in one cycle!")]
        [ShowInInspector, OdinSerialize]
        private List<BattleSide> _turnSequenceLoop = EnumExtensions.GetValues<BattleSide>().ToList();

        public BattleSide GetNextTurnSide(BattleSide currentTurnSide)
        {
            var i = _turnSequenceLoop.IndexOf(currentTurnSide);
            return _turnSequenceLoop[GetNextIndex(i)];
        }

        public BattleSide GetStartingSide()
        {
            return _turnSequenceLoop[0];
        }

        private int GetNextIndex(int currentIndex)
        {
            if (currentIndex == _turnSequenceLoop.Count - 1)
                return 0;
            return currentIndex + 1;
        }
    }
}
