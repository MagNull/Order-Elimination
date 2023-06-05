using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public interface IBattleObstacleSetup//IBattleCollision
    {
        /// <summary>
        /// Модифицирует точность стрельбы, проводящейся через данный объект.
        /// </summary>
        /// <param name="accuracy"> Функция точности. </param>
        /// <param name="shootingDirection"> Направление стрельбы. </param>
        /// <param name="smallestIntersectionSquare"> Площадь наименьшей части клетки, разделённой линией стрельбы (от 0 до 0.5). </param>
        /// <returns> Модифицированная функция точности. </returns>
        public bool IsAllowedToStay(AbilitySystemActor obstacleEntity, AbilitySystemActor entityToCheck);// +enter direction
        public bool IsAllowedToWalk(AbilitySystemActor obstacleEntity, AbilitySystemActor entityToCheck);// +walk direction
        public IContextValueGetter ModifyAccuracy(// +askingEntity
            IContextValueGetter accuracy, 
            double shootingAngle, 
            double smallestIntersectionSquare);
        //public bool IsObscuringView(AbilitySystemActor askingEntity, double viewAngle, double smallestIntersectionSquare);
    }

    public class EntityObstacleSetup : IBattleObstacleSetup
    {
        public bool IsAllowedToStay(AbilitySystemActor obstacleEntity, AbilitySystemActor entityToCheck)
        {
            return false;
        }

        public bool IsAllowedToWalk(AbilitySystemActor obstacleEntity, AbilitySystemActor entityToCheck)
        {
            var context = obstacleEntity.BattleContext;
            return context.GetRelationship(obstacleEntity.BattleSide, entityToCheck.BattleSide) == BattleRelationship.Ally;
        }

        public IContextValueGetter ModifyAccuracy(IContextValueGetter accuracy, double shootingAngle, double smallestIntersectionSquare)
        {
            return accuracy;
        }
    }

    public class BattleObstacleSetup : IBattleObstacleSetup
    {
        private struct AccuracyAffectionRule
        {
            [HideInInspector, OdinSerialize]
            private float _minSquare;

            //public float MinAffectedAngle { get; set; }
            //public float MaxAffectedAngle { get; set; }

            [ShowInInspector]
            public float MinSquareToAffect
            {
                get => _minSquare;
                set
                {
                    if (value < 0) value = 0;
                    if (value > 0.5) value = 0.5f;
                    _minSquare = value;
                }
            }

            [ShowInInspector, OdinSerialize]
            public MathOperation AccuracyOperation { get; private set; }

            [ShowInInspector, OdinSerialize]
            public IContextValueGetter ValueOperand { get; private set; }

            public IContextValueGetter ModifyValue(IContextValueGetter accuracy, double shootingAngle, double minIntersectionSquare)
            {
                if (MinSquareToAffect > minIntersectionSquare) return accuracy;
                var modifiedValue = new MathValueGetter
                {
                    Left = accuracy,
                    Operation = AccuracyOperation,
                    Right = ValueOperand
                };
                return modifiedValue;
            }
        }

        [PropertyOrder(-10)]
        [ShowInInspector, OdinSerialize]
        public bool AllowsToStay { get; private set; } = false;

        [PropertyOrder(-9)]
        [ShowInInspector, OdinSerialize]
        public bool AllowsToWalkOn { get; private set; } = false;

        [PropertyOrder(-8)]
        [ShowIf(nameof(AllowsToStay))]
        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _entityStayConditions = new();

        [PropertyOrder(-7)]
        [ShowIf(nameof(AllowsToWalkOn))]
        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _entityWalkConditions = new();

        [PropertyOrder(-6)]
        [ShowInInspector, OdinSerialize]
        private List<AccuracyAffectionRule> _accuracyAffectors = new();

        public bool IsAllowedToStay(AbilitySystemActor obstacleEntity, AbilitySystemActor entityToCheck)
        {
            if (!AllowsToStay) return false;
            return _entityStayConditions.All(c => c.IsConditionMet(
                obstacleEntity.BattleContext, obstacleEntity, entityToCheck));
        }

        public bool IsAllowedToWalk(AbilitySystemActor obstacleEntity, AbilitySystemActor entityToCheck)
        {
            if (!AllowsToWalkOn) return false;
            return _entityWalkConditions.All(c => c.IsConditionMet(
                obstacleEntity.BattleContext, obstacleEntity, entityToCheck));
        }

        public IContextValueGetter ModifyAccuracy(IContextValueGetter accuracy, double shootingAngle, double minIntersectionSquare)
        {
            var modifiedAccuracy = accuracy;
            foreach (var affector in _accuracyAffectors)
            {
                modifiedAccuracy = affector.ModifyValue(modifiedAccuracy, shootingAngle, minIntersectionSquare);
            }
            return modifiedAccuracy;
        }
    }
}
