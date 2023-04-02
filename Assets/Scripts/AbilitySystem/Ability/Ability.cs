namespace OrderElimination.AbilitySystem
{
    public class Ability
    {
        public AbilityView View { get; private set; }
        public AbilityGameRepresentation GameRepresentation { get; private set; }
        public AbilityRules Rules { get; private set; }
        public IAbilityCastSystem CastSystem { get; private set; }
        private AbilityExecution Execution;

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool Cast(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            if (!Rules.IsAbilityAvailable(battleContext, caster, GameRepresentation.Cost))
                return false;
            var casterPosition = battleContext.BattleMap.GetCellPosition(caster);
            var mapBorders = battleContext.BattleMap.CellRangeBorders;
            if (CastSystem is MultiTargetCastSystem multiTargetCastSystem)
            {
                var availableCells = Rules.GetAvailableCellPositions(battleContext, caster);
                multiTargetCastSystem.SetAvailableCellsForSelection(availableCells);
            }
            if (!CastSystem.StartTargeting(mapBorders, casterPosition))
                return false;
            CastSystem.TargetingConfirmed += onSelectionConfirmed;
            CastSystem.TargetingCanceled += onSelectionCanceled;
            return true;

            //Started casting. Now waiting until confirmation/cancellation.

            void onSelectionConfirmed(IAbilityCastSystem targetingSystem)
            {
                CastSystem.TargetingConfirmed -= onSelectionConfirmed;
                CastSystem.TargetingCanceled -= onSelectionCanceled;
                var executionGroups = CastSystem.ExtractCastTargetGroups();
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                Execution.Execute(abilityUseContext);
                CastSystem.CancelTargeting();
            }

            void onSelectionCanceled(IAbilityCastSystem targetingSystem)
            {
                CastSystem.TargetingConfirmed -= onSelectionConfirmed;
                CastSystem.TargetingCanceled -= onSelectionCanceled;
            }
        }
    }
}
