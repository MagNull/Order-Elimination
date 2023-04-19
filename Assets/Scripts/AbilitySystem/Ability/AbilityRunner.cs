using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        //event Unlocked

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool Cast(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            if (Cooldown > 0)
                return false;
            if (!AbilityData.Rules.IsAbilityAvailable(battleContext, caster, AbilityData.GameRepresentation.Cost))
                return false;
            var casterPosition = battleContext.BattleMap.GetPosition(caster);
            var mapBorders = battleContext.BattleMap.CellRangeBorders;
            if (AbilityData.TargetingSystem is MultiTargetTargetingSystem multiTargetCastSystem)
            {
                var availableCells = AbilityData.Rules.GetAvailableCellPositions(battleContext, caster);
                multiTargetCastSystem.SetAvailableCellsForSelection(availableCells);
            }
            if (!AbilityData.TargetingSystem.StartTargeting(mapBorders, casterPosition))
                return false;
            AbilityData.TargetingSystem.TargetingConfirmed += onConfirmed;
            AbilityData.TargetingSystem.TargetingCanceled += onCanceled;
            return true;

            //Started casting. Now waiting until confirmation/cancellation.

            void onConfirmed(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
                var executionGroups = AbilityData.TargetingSystem.ExtractCastTargetGroups();
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                AbilityData.Execution.Execute(abilityUseContext);
                AbilityData.TargetingSystem.CancelTargeting();
                Cooldown = AbilityData.GameRepresentation.CooldownTime;
            }

            void onCanceled(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
            }
        }
    }
}
