namespace OrderElimination.AbilitySystem
{
    public class BattleObstacle
    {
        private readonly IBattleObstacleSetup _setup;

        public AbilitySystemActor ObstacleEntity { get; }

        public BattleObstacle(IBattleObstacleSetup setup, AbilitySystemActor obstacleEntity)
        {
            _setup = setup;
            ObstacleEntity = obstacleEntity;
        }

        public bool IsAllowedToStay(AbilitySystemActor entityToCheck)
            => _setup.IsAllowedToStay(this, entityToCheck);

        public bool IsAllowedToWalk(AbilitySystemActor entityToCheck)
            => _setup.IsAllowedToWalk(this, entityToCheck);

        public ContextValueModificationResult ModifyAccuracy(
            IContextValueGetter accuracy, double shootingAngle, double minIntersectionSquare,
            AbilitySystemActor askingEntity)
            => _setup.ModifyAccuracy(accuracy, shootingAngle, minIntersectionSquare, this, askingEntity);
    }
}
