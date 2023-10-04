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
            CellGroupsContainer cellGroups,
            AbilitySystemActor actionMaker,
            AbilitySystemActor targetEntity,
            ActionCallOrigin calledFrom)
        {
            CalledFrom = calledFrom;
            BattleContext = battleContext;
            CellTargetGroups = cellGroups;
            ActionMaker = actionMaker;
            TargetEntity = targetEntity;
            //In some cases entity can be not on the map, but it's position is required.
            //(entity != null, position == null)
            //There must be option to pass it's last position
            //e.g. ZoneTrigger leaver (because of death)
            //Possible solutions:
            //1.New method overload with both entity and cell position (hard to control).
            //2.Maintain access to entity position in IBattleMap, even if entity is no longer on it.
            //3.Store last position somewhere (entity/bank/map).
            //Note: option 3 was chosen.
            if (targetEntity != null)
                TargetCell = battleContext.BattleMap.GetLastPosition(targetEntity);
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }

        public ActionContext(
            ActionCallOrigin calledFrom,
            IBattleContext battleContext,
            CellGroupsContainer cellGroups,
            AbilitySystemActor actionMaker)
        {
            CalledFrom = calledFrom;
            BattleContext = battleContext;
            CellTargetGroups = cellGroups;
            ActionMaker = actionMaker;
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }

        public ActionContext(
            ActionCallOrigin calledFrom,
            IBattleContext battleContext,
            CellGroupsContainer cellGroups,
            AbilitySystemActor actionMaker,
            Vector2Int targetCell)
        {
            CalledFrom = calledFrom;
            BattleContext = battleContext;
            CellTargetGroups = cellGroups;
            ActionMaker = actionMaker;
            TargetCell = targetCell;
            AnimationSceneContext = battleContext.AnimationSceneContext;
        }

        public static ActionContext CreateBest(
            ActionCallOrigin callOrigin,
            IBattleContext battleContext,
            CellGroupsContainer cellGroups,
            AbilitySystemActor actionMaker,
            Vector2Int? targetCell,
            AbilitySystemActor targetEntity)
        {
            if (targetEntity != null)
                return new ActionContext(battleContext, cellGroups, actionMaker, targetEntity, callOrigin);
            if (targetCell != null)
                return new ActionContext(callOrigin, battleContext, cellGroups, actionMaker, targetCell.Value);
            return new ActionContext(callOrigin, battleContext, cellGroups, actionMaker);
        }
    }
}
