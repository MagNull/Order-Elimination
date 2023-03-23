using OrderElimination.AbilitySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public struct ActionInstruction<TTarget> where TTarget : IActionTarget
    {
        //TargetCondition(враг/союзник/постройка/...), TargetHasEffectCondition, ...
        public ICommonCondition[] CommonConditions { get; set; } //доступность Action'а
        public ICellCondition[] CellConditions { get; set; } //доступность клетки
        public ITargetEntityCondition[] TargetEntityConditions { get; set; } //доступность сущности (для применения Action)
        
        public CellTargetGroupFilter TargetGroupsFilter { get; set; } //К каким группам клеткок будет применяться действие(Main,Area,...)
        public IBattleAction<TTarget> Action { get; set; }
        public int RepeatNumber { get; set; }
        public ActionInstruction<IBattleEntity>[] EntityInstructionsOnActionSuccess { get; set; }
        public ActionInstruction<Cell>[] CellInstructionsOnActionSuccess { get; set; }
        public bool PerformInstructionsOnSuccessForEveryEntity { get; set; } //??

        //TODO Заменить out-аргументы на объединяющий объект для вывода?
        public void GetActionsByTargets(CellTargetGroups selectedTargetGroups, out ActionsByTargets<IBattleEntity> entityActions, out ActionsByTargets<Cell> cellActions)
        {
            if (TTarget is IBattleEntity)
        }


    }

    public class ActionsByTargets<TTarget> where TTarget : IActionTarget
    {
        private Dictionary<TTarget, List<IBattleAction<TTarget>>> _actionsByTarget = new ();
        public IReadOnlyList<TTarget> Targets => _actionsByTarget.Keys.ToList();

        public IReadOnlyList<IBattleAction<TTarget>> GetActionsForTarget(TTarget target) => _actionsByTarget[target];

        public void AddActionForTarget(TTarget target, IBattleAction<TTarget> action)
        {
            if (!_actionsByTarget.ContainsKey(target))
                _actionsByTarget[target] = new List<IBattleAction<TTarget>>();
            _actionsByTarget[target].Add(action);
        }
}
}
