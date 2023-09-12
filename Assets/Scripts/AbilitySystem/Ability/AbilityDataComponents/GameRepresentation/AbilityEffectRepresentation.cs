namespace OrderElimination.AbilitySystem
{
    public class AbilityEffectRepresentation
    {
        public AbilityEffectRepresentation(IEffectData effectData, IContextValueGetter applyChance)
        {
            EffectData = effectData;
            ApplyChance = applyChance;
        }

        public IEffectData EffectData { get; }

        public IContextValueGetter ApplyChance { get; }
        //GetSimpleChanceValue
    }
}
