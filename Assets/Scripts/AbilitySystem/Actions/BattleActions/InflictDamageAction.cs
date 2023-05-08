using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.Infrastructure;
using Unity.Burst.CompilerServices;
using System.Linq;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    public class InflictDamageAction : BattleAction<InflictDamageAction>
    {
        private string _damageFormula => DamageSize.DisplayedFormula;
        private string _accuracyFormula => Accuracy.DisplayedFormula;


        [ShowInInspector, SerializeField]
        [PropertyTooltip("@" + nameof(_damageFormula)), GUIColor(1, 0.5f, 0.5f)]
        public IContextValueGetter DamageSize { get; set; }

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
        public IContextValueGetter Accuracy { get; set; }

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
        protected override InflictDamageAction ModifyAction(
            ActionContext context,
            bool actionMakerProcessing = true,
            bool targetProcessing = true)
        {
            if (context.ActionTarget == null)
                throw new System.ArgumentNullException();

            var modifiedAction = this;

            if (actionMakerProcessing && context.ActionMaker != null)
                modifiedAction = context.ActionMaker.ActionProcessor.ProcessOutcomingAction(modifiedAction);

            var modifiedAccuracy = modifiedAction.Accuracy;
            if (ObjectsBetweenAffectAccuracy && context.ActionMaker != null)
            {
                var battleMap = context.BattleContext.BattleMap;
                var intersections = CellMath.GetIntersectionBetween(
                    context.ActionMaker.Position, context.ActionTargetInitialPosition.Value);
                foreach (var intersection in intersections)
                {
                    foreach (var battleObstacle in battleMap
                        .GetVisibleEntities(intersection.CellPosition, context.BattleContext, context.ActionMaker.BattleSide)
                        .Select(e => e as IBattleObstacle)
                        .Where(o => o != null))
                    {
                        modifiedAccuracy = battleObstacle.ModifyAccuracy(modifiedAccuracy, intersection.IntersectionAngle, intersection.SmallestPartSquare);
                    }
                }
            }
            modifiedAction.Accuracy = modifiedAccuracy;

            if (targetProcessing && context.ActionTarget != null)
                modifiedAction = context.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction);
            return modifiedAction;
        }

        protected override async UniTask<bool> Perform(ActionContext useContext)
        {
            //Обработка объектов на линии огня (перенесена в ModifiedPerform)
            //Проверка шанса попадания (точность)
            var accuracy = Accuracy.GetValue(useContext);
            var evasion = IgnoreEvasion || !useContext.ActionTarget.BattleStats.HasParameter(BattleStat.Evasion)
                ? 0
                : useContext.ActionTarget.BattleStats.GetParameter(BattleStat.Evasion).ModifiedValue;
            var hitResult = useContext.BattleContext.HitCalculation.CalculateHitResult(accuracy, evasion);
            if (hitResult == HitResult.Success)
            {
                var damageSize = DamageSize.GetValue(useContext);
                var damageDealer = useContext.ActionMaker;
                var damageInfo = new DamageInfo(damageSize, ArmorMultiplier, HealthMultiplier, DamageType, DamagePriority, damageDealer);
                useContext.ActionTarget.TakeDamage(damageInfo);
                return true;
            }
            return false;
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
