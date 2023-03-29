namespace OrderElimination.AbilitySystem
{
    public class Ability
    {
        public AbilityView View { get; private set; } //name, icon, effects
        public AbilityGameRepresentation GameRepresentation { get; private set; }
        public AbilityConditions Conditions { get; private set; }
        public AbilityTargeting Targeting { get; private set; }
        private AbilityExecution Execution;

        //В идеале нужно передавать представление карты для конкретного игрока/стороны — для наведения.
        //Но при применении использовать реальный BattleMap (не представление)
        public bool Cast(IBattleContext battleContext, IAbilitySystemActor caster)
        {
            if (!Conditions.IsAbilityAvailable(battleContext, caster, GameRepresentation.Cost))
                return false;
            var availableCells = Conditions.GetAvailableCellPositions(battleContext, caster);
            var casterPosition = battleContext.BattleMap.GetCellPosition(caster);
            var mapBorders = battleContext.BattleMap.CellRangeBorders;
            if (!Targeting.StartSelection(availableCells, mapBorders, casterPosition))
                return false;
            Targeting.SelectionConfirmed += onSelectionConfirmed;
            Targeting.SelectionCanceled += onSelectionCanceled;
            return true;

            //Started casting. Now waiting until confirmation/cancellation.

            void onSelectionConfirmed(AbilityTargeting targetingSystem)
            {
                Targeting.SelectionConfirmed -= onSelectionConfirmed;
                Targeting.SelectionCanceled -= onSelectionCanceled;
                var executionGroups = Targeting.ExtractTargetedGroups();
                var abilityUseContext = new AbilityExecutionContext(battleContext, caster, executionGroups);
                Execution.Execute(abilityUseContext);
                Targeting.Cancel();
            }

            void onSelectionCanceled(AbilityTargeting targetingSystem)
            {
                Targeting.SelectionConfirmed -= onSelectionConfirmed;
                Targeting.SelectionCanceled -= onSelectionCanceled;
            }
        }
    }
}
