using OrderElimination.AbilitySystem.Animations;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "EffectPreset", menuName = "OrderElimination/AbilitySystem/Effect")]
    public sealed class EffectDataPreset : SerializedScriptableObject, IEffectData
    {
        private void _validateFunctionality()
        {
            if (!IsTemporary)
                TemporaryEffectFunctionaity = null;
            else if (TemporaryEffectFunctionaity == null)
                TemporaryEffectFunctionaity = new();
        }

        #region Visuals
        [TitleGroup("Visuals", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 0)]
        [PropertyOrder(0), ShowInInspector]
        private string _name
        {
            get => _view.Name;
            set => _view.Name = value;
        }

        [TitleGroup("Visuals")]
        [PropertyOrder(0), ShowInInspector]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        private Sprite _icon
        {
            get => _view.Icon;
            set => _view.Icon = value;
        }

        [TitleGroup("Visuals")]
        [PropertyOrder(0), ShowInInspector]
        private bool _isHidden
        {
            get => _view.IsHidden;
            set => _view.IsHidden = value;
        }

        [HideInInspector, OdinSerialize]
        private EffectView _view = new EffectView();
        public IReadOnlyEffectView View => _view;


        [TitleGroup("Visuals")]
        [GUIColor(0, 1, 1)]
        [ShowInInspector]
        private IAbilityAnimation _animationOnActivation
        {
            get => _view.AnimationOnActivation;
            set => _view.AnimationOnActivation = value;
        }

        [TitleGroup("Visuals")]
        [GUIColor(0, 1, 1)]
        [ShowInInspector]
        private IAbilityAnimation _animationOnDeactivation
        {
            get => _view.AnimationOnDeactivation;
            set => _view.AnimationOnDeactivation = value;
        }
        #endregion

        #region Rules
        [TitleGroup("Rules", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 1)]
        [EnumToggleButtons]
        [ShowInInspector, OdinSerialize]
        public EffectCharacter EffectCharacter { get; protected set; }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Stacking", ShowLabel = false)]
        [HorizontalGroup("Rules/Stacking/Horizontal", 0.7f)]
        [HideLabel, Title("Stacking Policy", Bold = false, HorizontalLine = false)]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5)]
        [ShowInInspector, OdinSerialize]
        public EffectStackingPolicy StackingPolicy { get; protected set; }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Stacking", ShowLabel = false)]
        [HorizontalGroup("Rules/Stacking/Horizontal")]
        [ShowIf("@" + nameof(StackingPolicy) + " == " + nameof(EffectStackingPolicy) + "." + nameof(EffectStackingPolicy.LimitedStacking))]
        [MinValue(1)]
        [HideLabel, Title("Max Stack Size", Bold = false, HorizontalLine = false)]
        [ShowInInspector, OdinSerialize]
        private int _maxStackSize { get; set; } = 1;

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Stacking", ShowLabel = false)]
        [HorizontalGroup("Rules/Stacking/Horizontal")]
        [ShowIf("@" + nameof(StackingPolicy) + " != " + nameof(EffectStackingPolicy) + "." + nameof(EffectStackingPolicy.LimitedStacking))]
        [HideLabel, Title("Max Stack Size", Bold = false, HorizontalLine = false)]
        [ShowInInspector]
        public int MaxStackSize
        {
            get
            {
                if (StackingPolicy == EffectStackingPolicy.OverrideOld
                    || StackingPolicy == EffectStackingPolicy.IgnoreNew)
                    return 1;
                if (StackingPolicy == EffectStackingPolicy.LimitedStacking)
                    return Mathf.Max(_maxStackSize, 1);
                return int.MaxValue;
            }
        }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Processing", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public bool UseApplierProcessing { get; protected set; }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Processing", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        public bool UseHolderProcessing { get; protected set; }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Removal", ShowLabel = false)]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [ShowInInspector, OdinSerialize]
        public bool IsTemporary { get; protected set; }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Removal", ShowLabel = false)]
        [EnableIf("@" + nameof(IsTemporary))]
        [SuffixLabel("rounds")]
        [ShowInInspector]
        private int Duration
        {
            get
            {
                if (TemporaryEffectFunctionaity != null)
                    return TemporaryEffectFunctionaity.ApplyingDuration;
                return -1;
            }
            set
            {
                if (!IsTemporary)
                    return;
                if (value < 1)
                    value = 1;
                if (TemporaryEffectFunctionaity == null)
                    TemporaryEffectFunctionaity = new();
                TemporaryEffectFunctionaity.ApplyingDuration = value;
            }
        }

        [TitleGroup("Rules")]
        [BoxGroup("Rules/Removal", ShowLabel = false)]
        [ShowInInspector, OdinSerialize]
        private HashSet<EffectTriggerAcceptor> _removeTriggers { get; set; } = new();

        public IEnumerable<EffectTriggerAcceptor> RemoveTriggers => _removeTriggers;
        #endregion

        #region Functionality
        [TitleGroup("Functionality", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 2)]
        [TabGroup("Functionality/Tabs", "Basic")]
        [GUIColor("@Color.green")]
        [PropertyOrder(-1)]
        [ShowInInspector, OdinSerialize]
        public IEffectInstruction InstructionOnActivation { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Basic")]
        [GUIColor(1, 0.35f, 0.35f)]
        [ShowInInspector, OdinSerialize]
        public IEffectInstruction InstructionOnDeactivation { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Basic")]
        [GUIColor("@Color.cyan")]
        [EnableIf("@" + nameof(IsTemporary))]
        [ShowInInspector]
        private IEffectInstruction InstructionOnTimeout
        {
            get => TemporaryEffectFunctionaity?.OnTimeOutInstruction;
            set
            {
                if (!IsTemporary)
                    return;
                if (TemporaryEffectFunctionaity == null)
                    TemporaryEffectFunctionaity = new();
                TemporaryEffectFunctionaity.OnTimeOutInstruction = value;
            }
        }

        [HideInInspector, OdinSerialize]
        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Processing")]
        [GUIColor(0.35f, 0.98f, 0.88f)]
        [ShowInInspector, OdinSerialize]
        public IActionProcessor IncomingActionProcessor { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Processing")]
        [GUIColor(0.88f, 0.35f, 0.98f)]
        [ShowInInspector, OdinSerialize]
        public IActionProcessor OutcomingActionProcessor { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Conditional")]
        [DictionaryDrawerSettings(KeyLabel = "Trigger", ValueLabel = "Instruction")]
        //[GUIColor(0.98f, 0.88f, 0.35f)]
        [GUIColor(1, 0.7f, 0.3f)]
        [PropertyOrder(-0.5f)]
        [ShowInInspector, OdinSerialize]
        private Dictionary<EffectTriggerAcceptor, IEffectInstruction> _triggerInstructions = new();
        public IReadOnlyDictionary<EffectTriggerAcceptor, IEffectInstruction> TriggerInstructions 
            => _triggerInstructions;
        #endregion

        public void OnActivation(BattleEffect effect)
        {
            var bank = effect.BattleContext.EntitiesBank;
            var targetView = bank.GetViewByEntity(effect.EffectHolder);
            var applier = effect.EffectApplier != null
                ? $"{bank.GetViewByEntity(effect.EffectApplier).Name}"
                : "<Unknown>";
            Logging.Log(
                $"Effect «{effect.EffectData.View.Name % Colorize.Orange}» has been applied on {targetView.Name % Colorize.Orange} by {applier % Colorize.Orange} during {effect.BattleContext.ActiveSide.ToString() % Colorize.Orange} turn.",
                context: this);
        }

        public void OnDeactivation(BattleEffect effect)
        {
            //var targetView = effect.BattleContext.EntitiesBank.GetViewByEntity(effect.EffectHolder);
            //Logging.Log($"Effect {effect.EffectData.View.Name} has been removed from {targetView.Name}.", Colorize.Orange, this);
        }
    }
}
