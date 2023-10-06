using OrderElimination.MacroGame;

namespace OrderElimination.AbilitySystem
{
    public class ValueCalculationContext
    {
        public static ValueCalculationContext Empty { get; } = new ValueCalculationContext();

        //private readonly Dictionary<int, float> _contextVariables = new();

        public readonly IBattleContext BattleContext;
        public readonly CellGroupsContainer CellTargetGroups;
        public readonly AbilitySystemActor BattleCaster;
        public readonly AbilitySystemActor BattleTarget;

        public readonly GameCharacter MetaCaster;//if no BattleCaster
        public readonly IGameCharacterTemplate TemplateCharacterCaster;//if no GameCaster
        public readonly IBattleStructureTemplate TemplateStructureCaster;//if no GameCaster/BattleCaster

        private ValueCalculationContext() { }

        private ValueCalculationContext(
            IBattleContext battleContext, 
            CellGroupsContainer cellTargetGroups, 
            AbilitySystemActor actionMaker, 
            AbilitySystemActor actionTarget)
        {
            BattleContext = battleContext;
            CellTargetGroups = cellTargetGroups;
            BattleCaster = actionMaker;
            BattleTarget = actionTarget;
        }

        private ValueCalculationContext(GameCharacter caster)
        {
            MetaCaster = caster;
        }

        private ValueCalculationContext(IGameCharacterTemplate casterTemplate)
        {
            TemplateCharacterCaster = casterTemplate;
        }

        public static ValueCalculationContext Full(
            IBattleContext battleContext,
            CellGroupsContainer cellTargetGroups,
            AbilitySystemActor actionMaker,
            AbilitySystemActor actionTarget)
            => new(battleContext, cellTargetGroups, actionMaker, actionTarget);

        public static ValueCalculationContext Full(ActionContext actionContext)
            => new(
                actionContext.BattleContext,
                actionContext.CellTargetGroups,
                actionContext.ActionMaker,
                actionContext.TargetEntity);

        public static ValueCalculationContext ForBattleCaster(
            IBattleContext battleContext,
            AbilitySystemActor actionMaker)
            => new(battleContext, null, actionMaker, null);

        public static ValueCalculationContext ForMetaCaster(GameCharacter caster)
            => new(caster);

        public static ValueCalculationContext ForMetaCaster(IGameCharacterTemplate caster)
            => new(caster);

        //public IReadOnlyDictionary<int, float> ContextVariables => _contextVariables;

        //public void WriteVariable(int id, float value)
        //{
        //    if (_contextVariables.ContainsKey(id))
        //        throw new InvalidOperationException("Variable with the same name has already been written.");
        //    _contextVariables.Add(id, value);
        //}
    }
}
