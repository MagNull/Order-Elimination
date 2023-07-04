using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Infrastructure;
using Unity.Burst.CompilerServices;
using System.Linq;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;

namespace OrderElimination.AbilitySystem
{
    public class InflictDamageAction : BattleAction<InflictDamageAction>
    {
        private string _damageFormula => DamageSize.DisplayedFormula;
        private string _accuracyFormula => Accuracy.DisplayedFormula;


        [ShowInInspector, SerializeField]
        [PropertyTooltip("@" + nameof(_damageFormula)), GUIColor(1, 0.5f, 0.5f)]
        public IContextValueGetter DamageSize { get; set; } = new CasterStatGetter(BattleStat.AttackDamage);

        [ShowInInspector, SerializeField]
        public float ArmorMultiplier { get; set; } = 1;

        [ShowInInspector, SerializeField]
        public float HealthMultiplier { get; set; } = 1;

        [ShowInInspector, SerializeField]
        public DamageType DamageType { get; set; }

        [ShowInInspector, SerializeField]
        public LifeStatPriority DamagePriority { get; set; }

        [ShowInInspector, SerializeField]
        [PropertyTooltip("@" + nameof(_accuracyFormula)), GUIColor(0.5f, 0.8f, 1f)]
        public IContextValueGetter Accuracy { get; set; } = new CasterStatGetter(BattleStat.Accuracy);

        [ShowInInspector, SerializeField]
        public bool IgnoreEvasion { get; set; }

        [ShowInInspector, SerializeField]
        public bool ObjectsBetweenAffectAccuracy { get; set; } //TODO Extract to Accuracy ValueGetter

        public override ActionRequires ActionRequires => ActionRequires.Entity;

        //[ShowInInspector, SerializeField]
        //public IDamageAnimation SuccessfulHitAnimation { get; private set; }
        //[ShowInInspector, SerializeField]
        //public IDamageAnimation FailedHitAnimation { get; private set; }

        //ConditionalDamageModifiers { get; set; } //по условиям цели изменяет наносимый урон
        //public bool IgnoreDeadTargets { get; set; }

        //*При вызове Perform IBattleAction уже обработан.
        protected override InflictDamageAction ProcessAction(
            ActionContext context,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            if (context.ActionTarget == null)
                Logging.LogException( new System.ArgumentNullException());

            var modifiedAction = this;

            if (actionMakerProcessing && context.ActionMaker != null)
                modifiedAction = context.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction, context);

            var modifiedAccuracy = modifiedAction.Accuracy;
            if (ObjectsBetweenAffectAccuracy && context.ActionMaker != null)
            {
                var battleMap = context.BattleContext.BattleMap;
                var intersections = CellMath.GetIntersectionBetween(
                    context.ActionMaker.Position, context.ActionTarget.Position);
                foreach (var intersection in intersections)
                {
                    foreach (var battleObstacle in context.BattleContext
                        .GetVisibleEntities(intersection.CellPosition, context.ActionMaker.BattleSide)
                        .Select(e => e.Obstacle))
                    {
                        modifiedAccuracy = battleObstacle.ModifyAccuracy(modifiedAccuracy, intersection.IntersectionAngle, intersection.SmallestPartSquare);
                    }
                }
            }
            modifiedAction.Accuracy = modifiedAccuracy;

            if (targetProcessing)
                modifiedAction = context.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction, context);
            return modifiedAction;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var accuracy = Accuracy.GetValue(useContext);
            var evasion = IgnoreEvasion || !useContext.ActionTarget.BattleStats.HasParameter(BattleStat.Evasion)
                ? 0
                : useContext.ActionTarget.BattleStats[BattleStat.Evasion].ModifiedValue;
            var hitResult = useContext.BattleContext.HitCalculation.CalculateHitResult(accuracy, evasion);
            var animationContext = new AnimationPlayContext(
                useContext.AnimationSceneContext,
                useContext.TargetCellGroups,
                useContext.ActionMaker,
                useContext.ActionTarget);
            if (hitResult == HitResult.Success)
            {
                var damageSize = DamageSize.GetValue(useContext);
                var damageDealer = useContext.ActionMaker;
                var damageInfo = new DamageInfo(damageSize, ArmorMultiplier, HealthMultiplier, DamageType, DamagePriority, damageDealer);
                useContext.ActionTarget.TakeDamage(damageInfo);
                return new SimplePerformResult(this, useContext, true);
            }
            else if (hitResult == HitResult.Miss)
            {
                await useContext.AnimationSceneContext.DefaultAnimations[DefaultAnimation.Miss].Play(animationContext);
            }
            else if (hitResult == HitResult.Evasion)
            {
                await useContext.AnimationSceneContext.DefaultAnimations[DefaultAnimation.Evasion].Play(animationContext);
            }
            return new SimplePerformResult(this, useContext, false);
        }

        public override IBattleAction Clone()
        {
            var clone = new InflictDamageAction();
            clone.DamageSize = DamageSize.Clone();
            clone.ArmorMultiplier = ArmorMultiplier;
            clone.HealthMultiplier = HealthMultiplier;
            clone.DamageType = DamageType;
            clone.DamagePriority = DamagePriority;
            clone.Accuracy = Accuracy.Clone();
            clone.IgnoreEvasion = IgnoreEvasion;
            clone.ObjectsBetweenAffectAccuracy = ObjectsBetweenAffectAccuracy;
            return clone;
        }
    }
}
