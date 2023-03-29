using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class ModifyBattleStatsAction : BattleAction<ModifyBattleStatsAction>
    {
        public BattleStat TargetedStat { get; set; }
        public Func<float, float> ValueChanger { get; set; }
        public int Duration { get; set; } //TODO restrict min/max duration

        protected override bool Perform(ActionUseContext useContext)
        {
            var valueProcessor = ValueChanger;
            var targetedStatCharacter = useContext.ActionTarget.BattleStats.GetParameter(TargetedStat);
            targetedStatCharacter.AddProcessor(valueProcessor);
            //SubscribeOnTrigger "Move (i+Duration) started"
            targetedStatCharacter.RemoveProcessor(valueProcessor);
            return true;
        }
    }
}
