﻿using Sirenix.Utilities;
using System;
using static OrderElimination.AbilitySystem.PassiveAbilityExecution;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityRunner
    {
        public PassiveAbilityRunner(IPassiveAbilityData abilityData)
        {
            AbilityData = abilityData;
        }

        private IPassiveExecutionActivationInfo _currentActivationInfo;

        public IPassiveAbilityData AbilityData { get; private set; }
        public bool IsActive { get; private set; } = false;
        public int Cooldown { get; private set; }

        //public event Action<ActiveAbilityRunner> AbilityCasted;
        //public event Action<ActiveAbilityRunner> AbilityCastCompleted;

        public void Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (IsActive) throw new InvalidOperationException("Passive Ability Runner has already been activated.");
            AbilityData.Execution.ExecutionTriggered += OnExecutionTriggered;
            _currentActivationInfo = AbilityData.Execution.Activate(battleContext, caster);
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) throw new InvalidOperationException("Passive Ability Runner is not active.");
            AbilityData.Execution.ExecutionTriggered -= OnExecutionTriggered;
            AbilityData.Execution.Dectivate(_currentActivationInfo);
            IsActive = false;
        }

        private void OnExecutionTriggered(IPassiveExecutionActivationInfo activationInfo)
        {
            if (_currentActivationInfo != activationInfo)
                return;
                //throw new ArgumentException(
                //    "Unknown activation info. One should be passed only to a Runner instance that created it.");
            if (AbilityData.GameRepresentation.CooldownTime > 0)
            {
                _currentActivationInfo.ActivationContext.NewRoundBegan += OnNewRound;
                Cooldown = AbilityData.GameRepresentation.CooldownTime;
                Deactivate();
            }
        }

        private void OnNewRound(IBattleContext battleContext)
        {
            Cooldown--;
            if (Cooldown <= 0)
            {
                Cooldown = 0;
                battleContext.NewRoundBegan -= OnNewRound;
                Activate(_currentActivationInfo.ActivationContext, _currentActivationInfo.Caster);
            }
        }
    }
}
