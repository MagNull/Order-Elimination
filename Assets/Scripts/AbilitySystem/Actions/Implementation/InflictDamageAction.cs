using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Infrastructure;
using Unity.Burst.CompilerServices;
using System.Linq;

namespace OrderElimination.AbilitySystem
{
    public class InflictDamageAction : BattleAction<InflictDamageAction>
    {
        //Меняться будет только значение урона
        //Заменить DamageInfo на отдельную информацию об уроне и формулу для получения значения?
        //В таком случае DamageInfo будет собираться только перед отправкой в target.TakeDamage()
        public DependentParameter<DamageInfo> DamageInfo { get; set; } 
        public DependentParameter<float> Accuracy { get; set; }
        public bool IgnoreEvasion { get; set; }
        public bool ObjectsAffectAccuracy { get; set; }
        //ConditionalDamageModifiers { get; set; } //по условиям цели изменяет наносимый урон
        //public bool IgnoreDeadTargets { get; set; }

        //*При вызове Perform IBattleAction уже обработан.
        public override InflictDamageAction GetModifiedAction(ActionUseContext useContext)
        {
            var modifiedAction = this;

            modifiedAction = useContext.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction);

            var modifiedAccuracy = modifiedAction.Accuracy;
            if (ObjectsAffectAccuracy)
            {
                var battleMap = useContext.BattleContext.BattleMap;
                var intersections = CellMath.GetIntersectionBetween(useContext.ActionMakerPosition, useContext.ActionTargetPosition);
                foreach (var intersection in intersections)
                {
                    foreach (var battleObstacle in battleMap[intersection.CellPosition]
                        .GetContainingEntities()
                        .Select(e => e as IBattleObstacle)
                        .Where(o => o != null))
                    {
                        modifiedAccuracy = battleObstacle.ModifyAccuracy(modifiedAccuracy, intersection.IntersectionAngle, intersection.SmallestPartSquare);
                    }
                }
            }
            modifiedAction.Accuracy = modifiedAccuracy;

            modifiedAction = useContext.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction);
            return modifiedAction;
        }

        protected override bool Perform(ActionUseContext useContext)
        {
            //Обработка объектов на линии огня

            //Проверка шанса попадания (точность)
            if (RandomExtensions.TryChance(Accuracy.GetValue(useContext)))
            {
                DamageInfo givenDamage;
                //TODO Apply ConditionalDamageModifiers
                //
                if (!IgnoreEvasion && useContext.ActionTarget.BattleStats.HasParameter(BattleStat.Evasion))
                {
                    var evasion = useContext.ActionTarget.BattleStats.GetParameter(BattleStat.Evasion);
                    if (RandomExtensions.TryChance(evasion.ModifiedValue))
                    {
                        useContext.ActionTarget.TakeDamage(DamageInfo.GetValue(useContext), out givenDamage);
                        return true;
                    }
                    return false; //evasion
                }
                useContext.ActionTarget.TakeDamage(DamageInfo.GetValue(useContext), out givenDamage);
                return true;
            }
            return false; //miss
        }
    }

    public class DamageInflictPerformResult : IBattleActionPerformResult<InflictDamageAction>
    {
        public InflictDamageAction Action => throw new System.NotImplementedException();
        public bool IsPerformedSuccessfully => throw new System.NotImplementedException();
        public DamageInfo DealtDamage => throw new System.NotImplementedException();
    }
}
