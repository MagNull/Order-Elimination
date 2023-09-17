namespace OrderElimination.AbilitySystem
{
    public class ApplyTemporaryArmorActionCallback : IBattleActionCallback
    {
        public ApplyTemporaryArmorActionCallback(TemporaryArmor removedArmor)
        {
            RemovedArmor = removedArmor;
        }

        public TemporaryArmor RemovedArmor { get; }
    }
}
