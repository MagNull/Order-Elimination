using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace OrderElimination.AbilitySystem
{
    public class ModifyBattleStatsAction : BattleAction<ModifyBattleStatsAction>, IUndoableBattleAction
    {
        public override ActionTargets ActionTargets => ActionTargets.EntitiesOnly;

        public BattleStat TargetedStat { get; set; } //Change Only In Inspector
        public Func<float, float> ValueChanger { get; set; } //Change Only In Inspector
        public int Duration { get; set; } //TODO restrict min/max duration

        private Dictionary<IAbilitySystemActor, ActionExecutionContext> _useContexts;

        protected override bool Perform(ActionExecutionContext useContext)
        {
            var valueProcessor = ValueChanger;
            var targetedStatCharacter = useContext.ActionTarget.BattleStats.GetParameter(TargetedStat);
            targetedStatCharacter.AddProcessor(valueProcessor);
            //SubscribeOnTrigger "Move (i+Duration) started"
            targetedStatCharacter.RemoveProcessor(valueProcessor);
            _useContexts.Add(useContext.ActionTarget, useContext);
            return true;
        }

        //TODO инстансинг BattleAction'ов при применении
        //Если BattleAction не будет дублироваться для каждой цели, смысла в этом методе нет.
        //Иначе, можно сделать словарь контекстов по сущностям.
        public bool Undo(IAbilitySystemActor undoTarget)
        {
            if (!_useContexts.ContainsKey(undoTarget))
                return false;
            return _useContexts[undoTarget].ActionTarget.BattleStats.GetParameter(TargetedStat).RemoveProcessor(ValueChanger);
        }
    }
}
