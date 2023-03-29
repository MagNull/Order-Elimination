using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class TargetingSystem //служит для выбора целевых клеток для способности из доступных
    {
        //bool Confirm()
        //CancelSelection()
        public UniTask<bool> StartTargeting(IBattleContext battleContext, ICellCondition[] cellConditions, IAbilitySystemActor caster)
        {
            var availableCells = new List<Cell>(); ;
            foreach (var cell in battleContext.BattleMap)
            {
                if (cellConditions.All(c => c.IsConditionMet(battleContext, caster, cell)))
                    availableCells.Add(cell);
            }
            while (true)
            {
                //Select if cell in availableCells (can't click unavailable cells)
                //ShowPreview
                //(Deselect)
                //...
                //Until Confirm() or Close()
                //UseAbility(selectionToTargetGroups)
            }
            throw new NotImplementedException();
        }

        public void OnCellClick(Cell cell) //OnCellClicked
        {

        }
    }
}
