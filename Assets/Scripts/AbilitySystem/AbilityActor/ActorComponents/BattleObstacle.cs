namespace OrderElimination.AbilitySystem
{
    public class BattleObstacle
    {
        public BattleObstacle(IBattleObstacleSetup setup, AbilitySystemActor obstacleEntity)
        {
            _setup = setup;
            _entity = obstacleEntity;
        }

        private readonly IBattleObstacleSetup _setup;
        private readonly AbilitySystemActor _entity;

        public bool IsAllowedToStay(AbilitySystemActor entityToCheck)
            => _setup.IsAllowedToStay(_entity, entityToCheck);

        public bool IsAllowedToWalk(AbilitySystemActor entityToCheck)
            => _setup.IsAllowedToWalk(_entity, entityToCheck);

        public IContextValueGetter ModifyAccuracy(IContextValueGetter accuracy, double shootingAngle, double minIntersectionSquare)
            => _setup.ModifyAccuracy(accuracy, shootingAngle, minIntersectionSquare);
    }
}
