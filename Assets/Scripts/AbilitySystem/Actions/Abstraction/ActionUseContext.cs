﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public struct ActionUseContext
    {
        public IBattleContext BattleContext { get; }

        //Вынесен в параметры вызова IBattleAction
        //public IBattleEntity actionMaker { get; }

        //Заменено разделением на действия с клетками и сущностями
        //public IActionTarget Target { get; }
        //public IBattleEntity EntityTarget => Target as IBattleEntity; //Сущность, с которой совершают действие
        //public Cell CellTarget => Target as Cell; //Клетка, с которой совершают действие

        public ActionUseContext(IBattleContext battleContext)//, IBattleEntity actionMaker, IActionTarget target)
        {
            BattleContext = battleContext;
            //ActionMaker = actionMaker;
            //Target = target;
        }
    }
}
