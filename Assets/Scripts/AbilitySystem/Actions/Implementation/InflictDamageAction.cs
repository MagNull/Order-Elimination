using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Infrastructure;
using Unity.Burst.CompilerServices;

namespace OrderElimination.AbilitySystem
{
    public class InflictDamageAction : EntityBattleAction
    {
        //�������� ����� ������ �������� �����
        //�������� DamageInfo �� ��������� ���������� �� ����� � ������� ��� ��������� ��������?
        //� ����� ������ DamageInfo ����� ���������� ������ ����� ��������� � target.TakeDamage()
        public DependentParameter<DamageInfo> DamageInfo { get; set; } 
        public DependentParameter<float> Accuracy { get; set; }
        //ConditionalDamageModifiers { get; set; }
        //public bool IgnoreDeadTargets { get; set; }

        public override bool Perform(ActionUseContext useContext, IBattleEntity actionMaker, IBattleEntity target)
        {
            //��������� �������� �� ����� ����
            var modifiedAccuracy = Accuracy.GetValue(useContext);
            var battleMap = useContext.BattleContext.BattleMap;
            foreach (var battleMapObject in battleMap.GetObjectsBetween(actionMaker, target))
            {
                modifiedAccuracy = battleMapObject.ModifyAccuracy(modifiedAccuracy, battleMap, actionMaker, target);
            }
            if (RandomExtensions.TryChanceFraction(modifiedAccuracy))
            {
                //Apply ConditionalDamageModifiers
                
                return target.TakeDamage(DamageInfo.GetValue(useContext)); //hit or evade
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
