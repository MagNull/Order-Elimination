namespace OrderElimination.AbilitySystem
{
    public enum EffectApplyResult
    {
        Success = 0,
        BlockedByImmunity,
        BlockedByStackingRules,
        BlockedByStackingLimit
    }
}
