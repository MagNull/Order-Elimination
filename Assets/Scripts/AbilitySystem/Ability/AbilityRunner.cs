using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace OrderElimination.AbilitySystem
{
    public class AbilityRunner
    {
        public AbilityRunner(AbilityData abilityData)
        {
            AbilityData = abilityData;
        }

        public AbilityData AbilityData { get; private set; }
        public int Cooldown { get; private set; }

        public Action<AbilityRunner> AbilityUsed;
        //event Unlocked

        public bool IsCastAvailable(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            return Cooldown <= 0 && AbilityData.Rules.IsAbilityAvailable(battleContext, caster);
        }

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool InitiateCast(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            if (!IsCastAvailable(battleContext, caster))
                return false;
            var casterPosition = battleContext.BattleMap.GetPosition(caster);
            var mapBorders = battleContext.BattleMap.CellRangeBorders;
            if (AbilityData.TargetingSystem is IRequireTargetsTargetingSystem targetingSystem)
            {
                var availableCells = AbilityData.Rules.GetAvailableCellPositions(battleContext, caster);
                targetingSystem.SetAvailableCellsForSelection(availableCells);
            }
            if (!AbilityData.TargetingSystem.StartTargeting(mapBorders, casterPosition))
                return false;
            AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
            AbilityData.TargetingSystem.TargetingConfirmed += onConfirmed;
            AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
            AbilityData.TargetingSystem.TargetingCanceled += onCanceled;
            return true;

            //Started casting. Now waiting until confirmation/cancellation.

            async void onConfirmed(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
                var executionGroups = AbilityData.TargetingSystem.ExtractCastTargetGroups();
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                caster.RemoveActionPoints(AbilityData.Rules.UsageCost);
                AbilityData.TargetingSystem.CancelTargeting();
                Cooldown = AbilityData.GameRepresentation.CooldownTime;
                battleContext.NewRoundStarted -= onNewRoundStarted;
                battleContext.NewRoundStarted += onNewRoundStarted;
                AbilityUsed?.Invoke(this);
                await AbilityData.Execution.Execute(abilityUseContext);
            }

            void onCanceled(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
            }

            void onNewRoundStarted(IBattleContext battleContext)
            {
                Cooldown--;
                if (Cooldown <= 0)
                {
                    Cooldown = 0;
                    battleContext.NewRoundStarted -= onNewRoundStarted;
                }
            }
        }
    }
}
