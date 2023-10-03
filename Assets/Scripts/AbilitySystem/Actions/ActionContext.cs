using OrderElimination.AbilitySystem.Animations;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class ActionContext
    {
        public IBattleContext BattleContext { get; }
        public CellGroupsContainer CellTargetGroups { get; }
        public AbilitySystemActor ActionMaker { get; }
        public Vector2Int? TargetCell { get; }
        public AbilitySystemActor TargetEntity { get; }
        //AbilityUseContext (+ initial caster position, initial target position)
        public ActionCallOrigin CalledFrom { get; }
        //CalledAbility - способность, инициирующая действия
        //CalledEffect - эффект, инициирующий действие
        public AnimationSceneContext AnimationSceneContext { get; }

        public ActionContext(
            IBattleContext battleContext,
            CellGroupsContainer cellTargetGroups,
            AbilitySystemActor actionMaker,
            AbilitySystemActor target,
            ActionCallOrigin calledFrom)
        {
            BattleContext = battleContext;
            CalledFrom = calledFrom;
            CellTargetGroups = cellTargetGroups;
            ActionMaker = actionMaker;
            TargetEntity = target;
            //In some cases entity can be not on the map, but it's position is required.
            //(entity != null, position == null)
            //There must be option to pass it's last position
            //e.g. ZoneTrigger leaver (because of death)
            //1.New method overload with both entity and cell position (hard to control).
            //2.Maintain access to entity position in IBattleMap, even if entity is no longer on it.
            //3.Store last position somewhere (entity/bank/map).
            //Note: option 3 was chosen.
            if (target != null)
                TargetCell = battleContext.BattleMap.GetLastPosition(target);
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }

        public ActionContext(
            IBattleContext battleContext,
            CellGroupsContainer cellTargetGroups,
            AbilitySystemActor actionMaker,
            Vector2Int target,
            ActionCallOrigin calledFrom)
        {
            BattleContext = battleContext;
            CalledFrom = calledFrom;
            CellTargetGroups = cellTargetGroups;
            ActionMaker = actionMaker;
            TargetCell = target;
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }
    }
}
