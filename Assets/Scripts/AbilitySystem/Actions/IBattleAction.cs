using OrderElimination.AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleAction // Заменит на АБС?
    {
        public event Action<IBattleAction> SuccessfullyPerformed;
        public bool IsTargetAcceptable(IActionTarget target);
        public bool Perform(IBattleContext battleContext, ActionUseContext useContext);
    }

    public interface ICellBattleAction
    {
        //Perform(..., cell)
    }

    public interface IEntityBattleAction
    {
        //Perform(..., entity)
    }

    public class InflictDamageAction : IEntityBattleAction
    {
        public DependentParameter<DamageInfo> DamageInfo { get; set; } //Меняться будет только значение урона
        public DependentParameter<float> HitChance { get; set; }

        public event Action<IBattleAction> SuccessfullyPerformed;

        public bool IsTargetAcceptable(IActionTarget target) => target is IBattleEntity;

        public bool Perform(IBattleContext battleContext, ActionUseContext useContext)
        {
            //Вынести вычисление случайного шанса в отдельную инфраструктурную логику?
            if (UnityEngine.Random.Range(0f, 100) <= HitChance.GetValue(useContext))
            {
                return useContext.EntityTarget.TakeDamage(DamageInfo.GetValue(useContext)); //hit or evade
            }
            else
                return false; //miss
        }
    }

    public class DamageInfo
    {
        //DamageType
        //IDependentParameter<float> DamageValue
    }

    public class ApplyEffectAction : IEntityBattleAction
    {
        public IEffect Effect { get; set; }

        public event Action<IBattleAction> SuccessfullyPerformed;

        public bool IsTargetAcceptable(IActionTarget target) => target is IBattleEntity;

        public bool Perform(IBattleContext battleContext, ActionUseContext useContext)
        {
            return useContext.EntityTarget.ApplyEffect(Effect);
        }
    }
}
