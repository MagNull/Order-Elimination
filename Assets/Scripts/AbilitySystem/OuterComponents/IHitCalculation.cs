using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class StandartHitCalculation : IHitCalculation
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
}
