using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    public class BattleObstacleSetup : IBattleObstacleSetup
    {
        private struct AccuracyAffectionRule
        {
            #region OdinVisuals
            [Tooltip("Affected squares (min border is exclusive)")]
            [ShowInInspector, MinMaxSlider(0, 0.5f, ShowFields = true)]
            private Vector2 _affectedSquare
            {
                get => new(_minAffectedSquare, _maxAffectedSquare);
                set
                {
                    if (value.y < value.x)
                        value.y = value.x;

                    _minAffectedSquare = value.x;
                    _maxAffectedSquare = value.y;
                }
            }

            [ShowInInspector, DisplayAsString]
            private string _accuracyFormula
            {
                get
                {
                    if (ValueOperand != null)
                        return $"=(InitialAccuracy {AccuracyOperation.AsString()} {ValueOperand.DisplayedFormula})";
                    return "No value operand.";
                }
            }
            #endregion

            [HideInInspector, OdinSerialize]
            private float _minAffectedSquare;
            [HideInInspector, OdinSerialize]
            private float _maxAffectedSquare;

            //public float MinAffectedAngle { get; set; }
            //public float MaxAffectedAngle { get; set; }
            
            [ShowInInspector, OdinSerialize]
            public MathOperation AccuracyOperation { get; private set; }

            [ShowInInspector, OdinSerialize]
            public IContextValueGetter ValueOperand { get; private set; }

            public ContextValueModificationResult ModifyValue(
                IContextValueGetter initialAccuracy, 
                double shootingAngle, 
                double minIntersectionSquare)
            {
                var isSquareInAffectedRange =
                    _minAffectedSquare < minIntersectionSquare
                    && minIntersectionSquare <= _maxAffectedSquare;
                if (!isSquareInAffectedRange)
                    return new(false, initialAccuracy);
                var modifiedAccuracy = new BinaryMathValueGetter
                {
                    Left = initialAccuracy,
                    Operation = AccuracyOperation.ToBinaryOperation(),
                    Right = ValueOperand
                };
                return new(true, modifiedAccuracy);
            }
        }

        [PropertyOrder(-10)]
        [GUIColor(0.8f, 1, 0)]
        [ShowInInspector, OdinSerialize]
        public bool AllowsToStay { get; private set; }

        [PropertyOrder(-9)]
        [GUIColor(1, 0.8f, 0)]
        [ShowInInspector, OdinSerialize]
        public bool AllowsToWalkOn { get; private set; }
        
        [PropertyOrder(-8)]
        [GUIColor(0.8f, 1, 0)]
        [ShowIf(nameof(AllowsToStay))]
        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _entityStayConditions = new();

        [PropertyOrder(-7)]
        [GUIColor(1, 0.8f, 0), PropertySpace(5)]
        [ShowIf(nameof(AllowsToWalkOn))]
        [ShowInInspector, OdinSerialize]
        private List<IEntityCondition> _entityWalkConditions = new();

        [PropertyOrder(-6)]
        [GUIColor(0.5f, 1, 1), PropertySpace(15)]
        [ShowInInspector, OdinSerialize]
        private List<AccuracyAffectionRule> _accuracyAffectors = new();

        public bool IsAllowedToStay(BattleObstacle obstacle, AbilitySystemActor entityToCheck)
        {
            if (!AllowsToStay) return false;
            var obstacleEntity = obstacle.ObstacleEntity;
            return _entityStayConditions.All(c => c.IsConditionMet(
                obstacleEntity.BattleContext, obstacleEntity, entityToCheck));
        }

        public bool IsAllowedToWalk(BattleObstacle obstacle, AbilitySystemActor entityToCheck)
        {
            if (!AllowsToWalkOn) return false;
            var obstacleEntity = obstacle.ObstacleEntity;
            return _entityWalkConditions.All(c => c.IsConditionMet(
                obstacleEntity.BattleContext, obstacleEntity, entityToCheck));
        }

        public ContextValueModificationResult ModifyAccuracy(
            IContextValueGetter initialAccuracy, 
            double shootingAngle, 
            double minIntersectionSquare,
            BattleObstacle obstacle,
            AbilitySystemActor askingEntity)
        {
            var anyModified = false;
            var modifiedAccuracy = initialAccuracy;
            foreach (var affector in _accuracyAffectors)
            {
                var mod = affector.ModifyValue(modifiedAccuracy, shootingAngle, minIntersectionSquare);
                if (mod.IsModificationSuccessful)
                {
                    modifiedAccuracy = mod.ModifiedValueGetter;
                    anyModified = true;
                }
            }
            return new(anyModified, modifiedAccuracy);
        }
    }
}
