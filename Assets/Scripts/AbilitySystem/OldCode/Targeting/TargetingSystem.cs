//using Cysharp.Threading.Tasks;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace OrderElimination.AbilitySystem
//{
//    public class TargetingSystem //служит для выбора целевых клеток для способности из доступных
//    {
//        public UniTask<bool> StartTargeting(IBattleContext battleContext, TargetFilter targetFilter)
//        {
//            var availableCells = GetAvailableCells(battleContext, targetFilter);
//            while (true)
//            {

//            }
//            throw new NotImplementedException();
//        }

//        public void OnSelectCell(Cell cell) //OnCellClicked
//        {

//        }

//        public Cell[] GetAvailableCells(IBattleContext battleContext, TargetFilter targetFilter)
//        {
//            var availableCells = new List<Cell>();
//            foreach (var cell in battleContext.BattleMapCells)
//            {
//                if (targetFilter.IsAccepted(cell)) 
//                    availableCells.Add(cell);
//            }
//            return availableCells.ToArray();
//        }
//    }
//}
