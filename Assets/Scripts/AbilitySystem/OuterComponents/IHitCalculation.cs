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
        public bool CalculateHitResult(float accuracy, float evasion, out HitResult hitResult);
    }

    //Replace with universalHitCalculation class (perfect accuracy, no evasion, perfect evasion, etc...)
    public class StandartHitCalculation : IHitCalculation
    {
        public bool CalculateHitResult(float accuracy, float evasion, out HitResult hitResult)
        {
            if (RandomExtensions.RollChance(accuracy))
            {
                if (RandomExtensions.RollChance(evasion))
                {
                    hitResult = HitResult.Evasion;
                    return false;
                }
                hitResult = HitResult.Success;
                return true;
            }
            hitResult = HitResult.Miss;
            return false;
        }
    }
}
