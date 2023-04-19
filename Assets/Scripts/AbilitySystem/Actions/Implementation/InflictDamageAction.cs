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
        //�������� ����� ������ �������� �����
        //�������� DamageInfo �� ��������� ���������� �� ����� � ������� ��� ��������� ��������?
        //� ����� ������ DamageInfo ����� ���������� ������ ����� ��������� � target.TakeDamage()
        public DependentParameter<DamageInfo> DamageInfo { get; set; } 
        public DependentParameter<float> Accuracy { get; set; }
        public bool IgnoreEvasion { get; set; }
        public bool ObjectsAffectAccuracy { get; set; }
        //ConditionalDamageModifiers { get; set; } //�� �������� ���� �������� ��������� ����
        //public bool IgnoreDeadTargets { get; set; }

        //*��� ������ Perform IBattleAction ��� ���������.
        public override InflictDamageAction GetModifiedAction(
            ActionExecutionContext useContext,
            bool actionMakerProcessing = false,
            bool targetProcessing = false)
        {
            var modifiedAction = this;

            if (actionMakerProcessing)
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

            if (targetProcessing)
                modifiedAction = useContext.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction);
            return modifiedAction;
        }

        protected override bool Perform(ActionExecutionContext useContext)
        {
            if (useContext.ActionTarget is not IHaveBattleLifeStats damageable)
                return false;
            //��������� �������� �� ����� ���� (���������� � ModifiedPerform)

            //�������� ����� ��������� (��������)
            var accuracy = Accuracy.GetValue(useContext);
            var evasion = IgnoreEvasion || !useContext.ActionTarget.BattleStats.HasParameter(BattleStat.Evasion)
                ? 0
                : useContext.ActionTarget.BattleStats.GetParameter(BattleStat.Evasion).ModifiedValue;
            var isSuccessful = useContext.BattleContext.HitCalculation.CalculateHitResult(accuracy, evasion, out var hitResult);
            if (isSuccessful)
            {
                //Calculate DamageInfo
                var damageInfo = DamageInfo.GetValue(useContext);
                useContext.ActionTarget.TakeDamage(damageInfo);
                return true;
            }
            return false;
        }
    }
}
