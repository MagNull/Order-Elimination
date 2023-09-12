using System;
namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityRunner
    {
        public ActiveAbilityRunner(IActiveAbilityData abilityData, AbilityProvider provider)
        {
            AbilityData = abilityData;
            AbilityProvider = provider;
        }

        public IActiveAbilityData AbilityData { get; }
        public AbilityProvider AbilityProvider { get; }

        public bool IsRunning { get; private set; } = false;
        public int Cooldown { get; private set; }

        public event Action<ActiveAbilityRunner> AbilityExecutionStarted;
        public event Action<ActiveAbilityRunner> AbilityExecutionCompleted;

        public bool IsCastAvailable(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (caster.IsDisposedFromBattle)
            {
                Logging.LogException(new InvalidOperationException("Caster is disposed from battle."));
                return false;
            }
            if (!caster.IsAlive)
            {
                Logging.LogException(new InvalidOperationException("Caster is dead."));
                return false;
            }
            return !IsRunning // :(
                && !caster.StatusHolder.HasStatus(BattleStatus.ActiveAbilitiesDisabled)
                && !caster.IsPerformingAbility
                && Cooldown <= 0 
                && AbilityData.Rules.IsAbilityAvailable(battleContext, caster);
        }

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool InitiateCast(IBattleContext battleContext, AbilitySystemActor caster)
        {
            if (!IsCastAvailable(battleContext, caster))
                return false;
            var casterPosition = caster.Position;
            var mapBorders = battleContext.BattleMap.CellRangeBorders;
            if (!AbilityData.TargetingSystem.StartTargeting(battleContext, caster))
                return false;
            AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
            AbilityData.TargetingSystem.TargetingConfirmed += onConfirmed;
            AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
            AbilityData.TargetingSystem.TargetingCanceled += onCanceled;
            return true;

            //Started targeting. Now waiting until confirmation/cancellation.

            async void onConfirmed(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
                var executionGroups = AbilityData.TargetingSystem.ExtractCastTargetGroups();
                AbilityData.TargetingSystem.CancelTargeting();
                caster.RemoveEnergyPoints(AbilityData.Rules.UsageCost);
                Cooldown = AbilityData.GameRepresentation.CooldownTime;
                battleContext.NewRoundBegan -= decreaseCooldown;
                battleContext.NewRoundBegan += decreaseCooldown;
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                IsRunning = true;
                caster.IsPerformingAbility = true;
                AbilityExecutionStarted?.Invoke(this);//Ability functionality initiated, but not finished yet.

                await AbilityData.Execution.Execute(abilityUseContext);

                IsRunning = false;
                caster.IsPerformingAbility = false;
                AbilityExecutionCompleted?.Invoke(this);
            }

            void onCanceled(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
            }

            void decreaseCooldown(IBattleContext battleContext)
            {
                Cooldown--;
                if (Cooldown <= 0)
                {
                    Cooldown = 0;
                    battleContext.NewRoundBegan -= decreaseCooldown;
                }
            }
        }
    }
}
