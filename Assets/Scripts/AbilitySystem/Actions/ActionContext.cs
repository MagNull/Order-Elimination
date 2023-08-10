using OrderElimination.AbilitySystem.Animations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActionContext
    {
        public readonly IBattleContext BattleContext;
        public readonly CellGroupsContainer TargetCellGroups;
        public readonly AbilitySystemActor ActionMaker;
        public readonly AbilitySystemActor ActionTarget;
        //AbilityUseContext (+ initial caster position, initial target position)

        public readonly AnimationSceneContext AnimationSceneContext;
        //CalledAbility - способность, инициирующая действия
        //CalledEffect - эффект, инициирующий действие

        public ActionContext(
            IBattleContext battleContext,
            CellGroupsContainer targetCellGroups,
            AbilitySystemActor actionMaker,
            AbilitySystemActor target)
        {
            BattleContext = battleContext;
            TargetCellGroups = targetCellGroups;
            ActionMaker = actionMaker;
            ActionTarget = target;
            //if (targetPosition == null && target != null && battleContext.BattleMap.ContainsEntity(target))
            //    targetPosition = target.Position;
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }
    }
}
