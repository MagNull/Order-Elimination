using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public struct ActionInstruction<TAction> where TAction : IBattleAction
    {
        //TargetCondition(враг/союзник/постройка/...), TargetHasEffectCondition, ...
        public ICommonCondition[] CommonConditions { get; set; } //доступность Action'а
        public ICellCondition[] CellConditions { get; set; } //доступность клетки
        public ITargetEntityCondition[] TargetEntityConditions { get; set; } //доступность сущности (для применения Action)
        
        public CellTargetGroupFilter TargetGroupsFilter { get; set; } //К каким группам клеткок будет применяться действие(Main,Area,...)
        public IBattleAction Action { get; set; }
        public int RepeatNumber { get; set; }
        public ActionInstruction[] InstructionsOnActionSuccess { get; set; }
        public bool PerformInstructionsOnSuccessForEveryEntity { get; set; }

        //public event Action<IBattleAction> PerformSucceeded;
        //public event Action<IBattleAction> PerformFailed;

        public List<IBattleAction> ProjectOnTarget(IActionTarget target)
        {
            //не содержит условия, содержит однородный список
            var actionsForTarget = new List<IBattleAction>();
            //filtering
            return actionsForTarget;
        }
    }
}
