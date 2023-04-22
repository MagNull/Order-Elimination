using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //Сделать частью IAbilitySystemActor?
    public interface IBattleObstacle
    {
        /// <summary>
        /// Модифицирует точность стрельбы, проводящейся через данный объект.
        /// </summary>
        /// <param name="accuracy"> Функция точности. </param>
        /// <param name="shootingDirection"> Направление стрельбы. </param>
        /// <param name="smallestIntersectionPartSquare"> Площадь наименьшей части клетки, разделённой линией стрельбы (от 0 до 0.5). </param>
        /// <returns> Модифицированная функция точности. </returns>
        public IContextDependentParameter<float> ModifyAccuracy(IContextDependentParameter<float> accuracy, double shootingAngle, double smallestIntersectionPartSquare);
    }
}
