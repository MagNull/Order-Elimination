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
            if (!IsProcessingIncomingAction)
                IncomingActionProcessor = null;
            if (!IsProcessingOutcomingAction)
                OutcomingActionProcessor = null;
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
        [ShowInInspector, OdinSerialize]
        public EffectStackingPolicy StackingPolicy { get; protected set; }

        [TitleGroup("Rules")]
        [ShowInInspector, OdinSerialize]
        public bool UseApplierProcessing { get; protected set; }

        [TitleGroup("Rules")]
        [ShowInInspector, OdinSerialize]
        public bool UseHolderProcessing { get; protected set; }

        [TitleGroup("Rules")]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [ShowInInspector, OdinSerialize]
        public bool IsTemporary { get; protected set; }

        [TitleGroup("Rules")]
        [ShowInInspector, OdinSerialize]
        private HashSet<EffectTriggerAcceptor> _removeTriggers = new();
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
        [ShowIf("@" + nameof(IsTemporary))]
        [ShowInInspector, OdinSerialize]
        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Processing")]
        [GUIColor(0.35f, 0.98f, 0.88f)]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [PropertyOrder(0f)]
        [ShowInInspector, OdinSerialize]
        public bool IsProcessingIncomingAction { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Processing")]
        [ShowIf("@" + nameof(IsProcessingIncomingAction))]
        [GUIColor(0.35f, 0.98f, 0.88f)]
        [ShowInInspector, OdinSerialize]
        public IActionProcessor IncomingActionProcessor { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Processing")]
        [GUIColor(0.88f, 0.35f, 0.98f)]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [ShowInInspector, OdinSerialize]
        public bool IsProcessingOutcomingAction { get; protected set; }

        [TitleGroup("Functionality")]
        [TabGroup("Functionality/Tabs", "Processing")]
        [GUIColor(0.88f, 0.35f, 0.98f)]
        [ShowIf("@" + nameof(IsProcessingOutcomingAction))]
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
            //var targetView = effect.BattleContext.EntitiesBank.GetViewByEntity(effect.EffectHolder);
            //Logging.Log($"Effect {effect.EffectData.View.Name} has been applied on {targetView.Name}.", Colorize.Orange, this);
        }

        public void OnDeactivation(BattleEffect effect)
        {
            //var targetView = effect.BattleContext.EntitiesBank.GetViewByEntity(effect.EffectHolder);
            //Logging.Log($"Effect {effect.EffectData.View.Name} has been removed from {targetView.Name}.", Colorize.Orange, this);
        }
    }
}
