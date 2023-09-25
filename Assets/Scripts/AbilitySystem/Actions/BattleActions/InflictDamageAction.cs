using UnityEngine;
using Sirenix.OdinInspector;
using Cysharp.Threading.Tasks;
using OrderElimination.AbilitySystem.Animations;

namespace OrderElimination.AbilitySystem
{
    public class InflictDamageAction : BattleAction<InflictDamageAction>
    {
        [ShowInInspector, SerializeField]
        [GUIColor(1, 0.5f, 0.5f)]
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
        [GUIColor(0.5f, 0.8f, 1f)]
        public IContextValueGetter Accuracy { get; set; } = new CasterStatGetter(BattleStat.Accuracy);

        [ShowInInspector, SerializeField]
        public bool IgnoreEvasion { get; set; }

        [LabelText("Obstacles Affect Accuracy")]
        [ShowInInspector, SerializeField]
        public bool ObjectsBetweenAffectAccuracy { get; set; } //TODO Extract to Accuracy ValueGetter

        public override ActionRequires ActionRequires => ActionRequires.Target;

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
                modifiedAccuracy = context.BattleContext.ModifyAccuracyBetween(
                    context.ActionMaker.Position,
                    context.ActionTarget.Position,
                    modifiedAccuracy,
                    context.ActionMaker);
            }
            modifiedAction.Accuracy = modifiedAccuracy;

            if (targetProcessing)
                modifiedAction = context.ActionTarget.ActionProcessor.ProcessIncomingAction(modifiedAction, context);
            return modifiedAction;
        }

        protected override async UniTask<IActionPerformResult> Perform(ActionContext useContext)
        {
            var calculationContext = ValueCalculationContext.Full(useContext);
            var accuracy = Accuracy.GetValue(calculationContext);
            var evasion = useContext.ActionTarget.BattleStats[BattleStat.Evasion].ModifiedValue;
            var hitResult = useContext.BattleContext.BattleRules.HitCalculation.CalculateHitResult(accuracy, evasion);
            var animationContext = new AnimationPlayContext(
                useContext.AnimationSceneContext,
                useContext.CellTargetGroups,
                useContext.ActionMaker,
                useContext.ActionTarget);
            if (hitResult == HitResult.Success
                || hitResult == HitResult.Evasion && IgnoreEvasion)
            {
                var damageInfo = CalculateDamage(useContext);
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

        public DamageInfo CalculateDamage(ActionContext useContext)
        {
            var calculationContext = ValueCalculationContext.Full(useContext);
            var damageSize = DamageSize.GetValue(calculationContext);
            var damageDealer = useContext.ActionMaker;
            var fromEffect = useContext.CalledFrom == ActionCallOrigin.Effect;
            var damageInfo = new DamageInfo(
                damageSize, ArmorMultiplier, HealthMultiplier, DamageType, DamagePriority, damageDealer, fromEffect);
            return damageInfo;
        }

        public float CalculateAccuracy(ActionContext useContext)
        {
            var calculationContext = ValueCalculationContext.Full(useContext);
            return Accuracy.GetValue(calculationContext);
        }
    }
}
