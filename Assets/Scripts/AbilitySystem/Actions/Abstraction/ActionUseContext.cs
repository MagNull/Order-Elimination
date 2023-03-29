using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActionUseContext
    {
        public readonly IBattleContext BattleContext;
        public readonly IAbilitySystemActor ActionMaker;
        public readonly IAbilitySystemActor ActionTarget;

        public readonly Vector2Int ActionMakerPosition;
        public readonly Vector2Int ActionTargetPosition;

        //Вынесен в параметры вызова IBattleAction
        //public IBattleEntity actionMaker { get; }

        //Заменено разделением на действия с клетками и сущностями
        //public IActionTarget Target { get; }
        //public IBattleEntity EntityTarget => Target as IBattleEntity; //Сущность, с которой совершают действие
        //public Cell CellTarget => Target as Cell; //Клетка, с которой совершают действие

        public ActionUseContext(IBattleContext battleContext, IAbilitySystemActor actionMaker, IAbilitySystemActor target)
        {
            BattleContext = battleContext;
            ActionMaker = actionMaker;
            ActionTarget = target;
            ActionMakerPosition = battleContext.BattleMap.GetCellPosition(actionMaker);
            ActionTargetPosition = battleContext.BattleMap.GetCellPosition(target);
        }
    }
}
