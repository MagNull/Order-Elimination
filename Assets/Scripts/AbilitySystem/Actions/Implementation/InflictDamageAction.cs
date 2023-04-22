using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Infrastructure;
using Unity.Burst.CompilerServices;
using System.Linq;
using Sirenix.OdinInspector;

namespace OrderElimination.AbilitySystem
{
    public class InflictDamageAction : BattleAction<InflictDamageAction>
    {
        public override ActionTargets ActionTargets => ActionTargets.EntitiesOnly;

        //Меняться будет только значение урона
        //Заменить DamageInfo на отдельную информацию об уроне и формулу для получения значения?
        //В таком случае DamageInfo будет собираться только перед отправкой в target.TakeDamage()
        [ShowInInspector, SerializeField]
        public IContextDependentParameter<float> DamageSize { get; private set; }

        [ShowInInspector, SerializeField]
        public float ArmorMultiplier { get; private set; } = 1;

        [ShowInInspector, SerializeField]
        public float HealthMultiplier { get; private set; } = 1;

        [ShowInInspector, SerializeField]
        public DamageType DamageType { get; private set; }

        [ShowInInspector, SerializeField]
        public DamagePriority DamagePriority { get; private set; }

        [ShowInInspector, SerializeField]
        public IContextDependentParameter<float> Accuracy { get; private set; }

        [ShowInInspector, SerializeField]
        public bool IgnoreEvasion { get; set; }

        [ShowInInspector, SerializeField]
        public bool ObjectsAffectAccuracy { get; set; }
        //ConditionalDamageModifiers { get; set; } //по условиям цели изменяет наносимый урон
        //public bool IgnoreDeadTargets { get; set; }

        //*При вызове Perform IBattleAction уже обработан.
        public override InflictDamageAction GetModifiedAction(
            ActionExecutionContext useContext,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            var modifiedAction = this;

            if (actionMakerProcessing && useContext.ActionMaker != null)
                modifiedAction = useContext.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction);

            var modifiedAccuracy = modifiedAction.Accuracy;
            if (ObjectsAffectAccuracy)
            {
                var battleMap = useContext.BattleContext.BattleMap;
                var intersections = CellMath.GetIntersectionBetween(useContext.ActionMakerPosition, useContext.ActionTargetPosition);
                foreach (var intersection in intersections)
                {
                    foreach (var battleObstacle in battleMap
                        .GetContainingEntities(intersection.CellPosition)
                        .Select(e => e as IBattleObstacle)
                        .Where(o => o != null))
                    {
                        modifiedAccuracy = battleObstacle.ModifyAccuracy(modifiedAccuracy, intersection.IntersectionAngle, intersection.SmallestPartSquare);
                    }
                }
            }
            modifiedAction.Accuracy = modifiedAccuracy;

            if (targetProcessing && useContext.ActionTarget != null)
                modifiedAction = useContext.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction);
            return modifiedAction;
        }

        protected override bool Perform(ActionExecutionContext useContext)
        {
            if (useContext.ActionTarget is not IHaveBattleLifeStats damageable)
                return false;
            //Обработка объектов на линии огня (перенесена в ModifiedPerform)

            //Проверка шанса попадания (точность)
            var accuracy = Accuracy.GetValue(useContext);
            var evasion = IgnoreEvasion || !useContext.ActionTarget.BattleStats.HasParameter(BattleStat.Evasion)
                ? 0
                : useContext.ActionTarget.BattleStats.GetParameter(BattleStat.Evasion).ModifiedValue;
            var hitResult = useContext.BattleContext.HitCalculation.CalculateHitResult(accuracy, evasion);
            if (hitResult == HitResult.Success)
            {
                //Calculate DamageInfo
                var damageSize = DamageSize.GetValue(useContext);
                var damageInfo = new DamageInfo(damageSize, ArmorMultiplier, HealthMultiplier, DamageType, DamagePriority);
                useContext.ActionTarget.TakeDamage(damageInfo);
                return true;
            }
            return false;
        }
    }
}
