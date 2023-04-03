namespace OrderElimination.AbilitySystem
{
    public class Ability
    {
        public AbilityView View { get; private set; }
        public AbilityGameRepresentation GameRepresentation { get; private set; }
        public AbilityRules Rules { get; private set; }
        public IAbilityTargetingSystem TargetingSystem { get; private set; }
        private AbilityExecution Execution;

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool Cast(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            if (!Rules.IsAbilityAvailable(battleContext, caster, GameRepresentation.Cost))
                return false;
            var casterPosition = battleContext.BattleMap.GetCellPosition(caster);
            var mapBorders = battleContext.BattleMap.CellRangeBorders;
            if (TargetingSystem is MultiTargetTargetingSystem multiTargetCastSystem)
            {
                var availableCells = Rules.GetAvailableCellPositions(battleContext, caster);
                multiTargetCastSystem.SetAvailableCellsForSelection(availableCells);
            }
            if (!TargetingSystem.StartTargeting(mapBorders, casterPosition))
                return false;
            TargetingSystem.TargetingConfirmed += onSelectionConfirmed;
            TargetingSystem.TargetingCanceled += onSelectionCanceled;
            return true;

            //Started casting. Now waiting until confirmation/cancellation.

            void onSelectionConfirmed(IAbilityTargetingSystem targetingSystem)
            {
                TargetingSystem.TargetingConfirmed -= onSelectionConfirmed;
                TargetingSystem.TargetingCanceled -= onSelectionCanceled;
                var executionGroups = TargetingSystem.ExtractCastTargetGroups();
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                Execution.Execute(abilityUseContext);
                TargetingSystem.CancelTargeting();
            }

            void onSelectionCanceled(IAbilityTargetingSystem targetingSystem)
            {
                TargetingSystem.TargetingConfirmed -= onSelectionConfirmed;
                TargetingSystem.TargetingCanceled -= onSelectionCanceled;
            }
        }
    }
}
