namespace OrderElimination.AbilitySystem
{
    public enum EffectStackingPolicy
    {
        UnlimitedStacking,
        IgnoreNew,
        OverrideOld,
        LimitedStacking//>>with ignoring new<< + with overriding oldest
        //ApplyAfterOld //Temporary effects only
        //SumDuration //Temporary effects only
    }
}
