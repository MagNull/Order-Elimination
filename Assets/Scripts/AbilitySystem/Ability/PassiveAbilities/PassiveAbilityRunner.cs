using Sirenix.Utilities;
using System;
using static OrderElimination.AbilitySystem.PassiveAbilityExecution;

namespace OrderElimination.AbilitySystem
{
    public class PassiveAbilityRunner
    {
        public PassiveAbilityRunner(IPassiveAbilityData abilityData, AbilityProvider provider)
        {
            AbilityData = abilityData;
            AbilityProvider = provider;
        }

        private IPassiveExecutionActivationInfo _currentActivationInfo;

        public IPassiveAbilityData AbilityData { get; }
        public AbilityProvider AbilityProvider { get; }
        public bool IsActive { get; private set; } = false;
        public int Cooldown { get; private set; }

        //public event Action<ActiveAbilityRunner> AbilityCasted;
        //public event Action<ActiveAbilityRunner> AbilityCastCompleted;

        public void Activate(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (IsActive)
                Logging.LogException(
                    new InvalidOperationException("Passive Ability Runner has already been activated."));
            AbilityData.Execution.ExecutionTriggered += OnExecutionTriggered;
            _currentActivationInfo = AbilityData.Execution.Activate(battleContext, caster);
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) Logging.LogException(new InvalidOperationException("Passive Ability Runner is not active."));
            AbilityData.Execution.Dectivate(_currentActivationInfo);
            AbilityData.Execution.ExecutionTriggered -= OnExecutionTriggered;
            IsActive = false;
        }

        private void OnExecutionTriggered(IPassiveExecutionActivationInfo activationInfo)
        {
            if (_currentActivationInfo != activationInfo)
                return;
            //Logging.LogException( new ArgumentException(
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