using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace OrderElimination.AbilitySystem
{
    public enum HitResult
    {
        Success,
        Miss,
        Evasion
    }

    public interface IHitCalculation
    {
        public HitResult CalculateHitResult(float accuracy, float evasion);
    }

    //Replace with universalHitCalculation class (perfect accuracy, no evasion, perfect evasion, etc...)
    public class StandardHitCalculation : IHitCalculation
    {
        public HitResult CalculateHitResult(float accuracy, float evasion)
        {
            if (RandomExtensions.RollChance(accuracy))
            {
                if (RandomExtensions.RollChance(evasion))
                {
                    return HitResult.Evasion;
                }
                return HitResult.Success;
            }
            return HitResult.Miss;
        }
    }

    public class ConfigurableHitCalculation : IHitCalculation
    {
        public enum RollOutcome
        {
            UseDefaultRoll,
            AlwaysTrue,
            AlwaysFalse
        }

        [ShowInInspector, OdinSerialize]
        public RollOutcome AccuracyRoll { get; set; } = RollOutcome.AlwaysTrue;

        [ShowInInspector, OdinSerialize]
        public RollOutcome EvasionRoll { get; set; } = RollOutcome.AlwaysFalse;

        public HitResult CalculateHitResult(float accuracy, float evasion)
        {
            if (AccuracyRoll == RollOutcome.AlwaysTrue
                || AccuracyRoll == RollOutcome.UseDefaultRoll && RandomExtensions.RollChance(accuracy))
            {
                if (EvasionRoll == RollOutcome.AlwaysTrue
                || EvasionRoll == RollOutcome.UseDefaultRoll && RandomExtensions.RollChance(evasion))
                {
                    return HitResult.Evasion;
                }
                return HitResult.Success;
            }
            return HitResult.Miss;
        }
    }
}
