﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace OrderElimination.AbilitySystem
{
    public class ActiveAbilityRunner
    {
        public ActiveAbilityRunner(IActiveAbilityData abilityData)
        {
            AbilityData = abilityData;
        }

        public IActiveAbilityData AbilityData { get; private set; }

        public bool IsRunning { get; private set; } = false;
        public int Cooldown { get; private set; }

        public event Action<ActiveAbilityRunner> AbilityInitiated;
        public event Action<ActiveAbilityRunner> AbilityCastCompleted;

        public bool IsCastAvailable(IBattleContext battleContext, AbilitySystemActor caster)
        {
            return !IsRunning // :(
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

            //Started targeting. Now waiting until confirmation/cancellation.

            async void onConfirmed(IAbilityTargetingSystem targetingSystem)
            {
                AbilityData.TargetingSystem.TargetingConfirmed -= onConfirmed;
                AbilityData.TargetingSystem.TargetingCanceled -= onCanceled;
                var executionGroups = AbilityData.TargetingSystem.ExtractCastTargetGroups();
                AbilityData.TargetingSystem.CancelTargeting();
                caster.RemoveActionPoints(AbilityData.Rules.UsageCost);
                Cooldown = AbilityData.GameRepresentation.CooldownTime;
                battleContext.NewRoundBegan -= decreaseCooldown;
                battleContext.NewRoundBegan += decreaseCooldown;
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                IsRunning = true;
                caster.IsPerformingAbility = true;
                AbilityInitiated?.Invoke(this);//Ability functionality initiated, but not finished yet.

                await AbilityData.Execution.Execute(abilityUseContext);

                IsRunning = false;
                caster.IsPerformingAbility = false;
                AbilityCastCompleted?.Invoke(this);
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
