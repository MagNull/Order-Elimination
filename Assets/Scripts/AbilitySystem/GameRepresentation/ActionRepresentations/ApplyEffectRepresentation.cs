namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class ApplyEffectRepresentation
    {
        public ApplyEffectRepresentation(IEffectData effectData, IContextValueGetter applyChance)
        {
            EffectData = effectData;
            ApplyChance = applyChance;
        }

        public IEffectData EffectData { get; }

        public IContextValueGetter ApplyChance { get; }

        //Target
    }
}
