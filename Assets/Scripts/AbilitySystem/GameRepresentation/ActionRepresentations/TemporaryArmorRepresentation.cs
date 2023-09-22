namespace OrderElimination.AbilitySystem.GameRepresentation
{
    public class TemporaryArmorRepresentation
    {
        public TemporaryArmorRepresentation(ApplyTemporaryArmorAction modifyStatsAction)
        {
            UnprocessedAction = modifyStatsAction;
        }

        public IContextValueGetter UnprocessedArmorAmount => UnprocessedAction?.TemporaryArmorAmount;
        public ApplyTemporaryArmorAction UnprocessedAction { get; }
    }
}
