﻿using OrderElimination.AbilitySystem.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public readonly struct ActionContext
    {
        public readonly IBattleContext BattleContext;
        public readonly CellGroupsContainer TargetCellGroups;
        public readonly AbilitySystemActor ActionMaker;
        public readonly AbilitySystemActor ActionTarget;

        public readonly Vector2Int? ActionTargetInitialPosition;

        public readonly AnimationSceneContext AnimationSceneContext;

        public AbilitySystemActor GetActionEntity(ActionEntity actionEntity)
        {
            return actionEntity switch
            {
                ActionEntity.Caster => ActionMaker,
                ActionEntity.Target => ActionTarget,
                _ => throw new NotImplementedException(),
            };
        }

        //Заменено разделением на действия с клетками и сущностями
        //public IActionTarget Target { get; }
        //public IBattleEntity EntityTarget => Target as IBattleEntity; //Сущность, с которой совершают действие
        //public Cell CellTarget => Target as Cell; //Клетка, с которой совершают действие

        public ActionContext(
            IBattleContext battleContext,
            CellGroupsContainer targetCellGroups,
            AbilitySystemActor actionMaker = null,
            AbilitySystemActor target = null,
            Vector2Int? targetPosition = null)
        {
            BattleContext = battleContext;
            TargetCellGroups = targetCellGroups;
            ActionMaker = actionMaker;
            ActionTarget = target;
            if (targetPosition == null && target != null)
                targetPosition = battleContext.BattleMap.GetPosition(target);
            ActionTargetInitialPosition = targetPosition;
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }
    }
}
