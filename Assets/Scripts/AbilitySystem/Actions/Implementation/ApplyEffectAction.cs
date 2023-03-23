using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ApplyEffectAction : EntityBattleAction
    {
        public IEffect Effect { get; set; }
        public float ApplyFractionChance { get; set; }

        public override bool Perform(ActionUseContext useContext, IBattleEntity actionMaker, IBattleEntity target)
        {
            if (RandomExtensions.TryChanceFraction(ApplyFractionChance))
            {
                return target.ApplyEffect(Effect); 
            }
            return false;
        }
    }
}
