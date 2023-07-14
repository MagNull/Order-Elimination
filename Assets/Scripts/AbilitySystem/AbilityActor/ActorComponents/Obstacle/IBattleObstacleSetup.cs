namespace OrderElimination.AbilitySystem
{
    public interface IBattleObstacleSetup
    {
        public bool IsAllowedToStay(BattleObstacle obstacleEntity, AbilitySystemActor entityToCheck);// +enter direction

        public bool IsAllowedToWalk(BattleObstacle obstacleEntity, AbilitySystemActor entityToCheck);// +walk direction

        /// <summary>
        /// Модифицирует точность стрельбы, проводящейся через данный объект.
        /// </summary>
        /// <param name="accuracy"> Функция точности. </param>
        /// <param name="shootingDirection"> Направление стрельбы. </param>
        /// <param name="smallestIntersectionSquare"> Площадь наименьшей части клетки, разделённой линией стрельбы (от 0 до 0.5). </param>
        /// <returns> Модифицированная функция точности. </returns>
        public ContextValueModificationResult ModifyAccuracy(// +askingEntity
            IContextValueGetter accuracy, 
            double shootingAngle, 
            double smallestIntersectionSquare,
            BattleObstacle obstacle,
            AbilitySystemActor askingEntity);
        //public bool IsObscuringView(AbilitySystemActor askingEntity, double viewAngle, double smallestIntersectionSquare);
    }
}
