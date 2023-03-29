using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public interface ISelectionSystemOld //SingleTargetSelection, NoTargetSelection, MultiTargetSelection
    {
        public event Action SelectionAchieved;
        public event Action SelectionLost; //SelectionMissed/SelectionLost/SelectionFellBack – Выделение снова не выполнено/перестало быть допустимым
        public bool IsSelectionAchieved { get; }
        public Cell[] GetSelection();
        public void AddToSelection(Cell cell);
        public void RemoveFromSelection(Cell cell);
        //public void StartSelecton(Cell[] availableCells);
        //Вынести (не относится к нацеливанию игрока), но вызывается после выбора им цели
        //public CellTargetGroups GetAffectedCellsForTarget(Cell targetCell); //Показывает затрагиваемые клетки при выборе новой цели
    }

    
}
