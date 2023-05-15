using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "EffectPreset", menuName = "AbilitySystem/Effect")]
    public class EffectDataPreset : SerializedScriptableObject, IEffectData
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
        [ShowInInspector, OdinSerialize]
        public bool CanBeForceRemoved { get; protected set; }

        #region Optionals
        [TitleGroup("Rules")]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [ShowInInspector, OdinSerialize]
        public bool IsTemporary { get; protected set; }

        [TitleGroup("Rules")]
        [ShowIf("@" + nameof(IsTemporary))]
        [ShowInInspector, OdinSerialize]
        public TemporaryEffectFunctionaity TemporaryEffectFunctionaity { get; protected set; }

        [TitleGroup("Functionality", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 2)]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _onActivationInstruction;

        [TitleGroup("Functionality")]
        [ShowInInspector, OdinSerialize]
        private IEffectInstruction _onDeactivationInstruction;

        [TitleGroup("Functionality")]
        [GUIColor(0.35f, 0.98f, 0.88f)]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [ShowInInspector, OdinSerialize]
        public bool IsProcessingIncomingAction { get; protected set; }

        [TitleGroup("Functionality")]
        [ShowIf("@" + nameof(IsProcessingIncomingAction))]
        [GUIColor(0.35f, 0.98f, 0.88f)]
        [ShowInInspector, OdinSerialize]
        public IActionProcessor IncomingActionProcessor { get; protected set; }

        [TitleGroup("Functionality")]
        [GUIColor(0.88f, 0.35f, 0.98f)]
        [OnValueChanged("@" + nameof(_validateFunctionality) + "()")]
        [ShowInInspector, OdinSerialize]
        public bool IsProcessingOutcomingAction { get; protected set; }

        [TitleGroup("Functionality")]
        [GUIColor(0.88f, 0.35f, 0.98f)]
        [ShowIf("@" + nameof(IsProcessingOutcomingAction))]
        [ShowInInspector, OdinSerialize]
        public IActionProcessor OutcomingActionProcessor { get; protected set; }

        [TitleGroup("Functionality")]
        [GUIColor(0.98f, 0.88f, 0.35f)]
        [ShowInInspector, OdinSerialize]
        private Dictionary<ITriggerSetupInfo, IEffectInstruction> _triggerInstructions = new();
        public IReadOnlyDictionary<ITriggerSetupInfo, IEffectInstruction> TriggerInstructions 
            => _triggerInstructions;
        #endregion

        public virtual void OnActivation(BattleEffect effect)
        {
            var targetView = effect.BattleContext.EntitiesBank.GetViewByEntity(effect.EffectHolder);
            Debug.Log($"Effect {effect.EffectData.View.Name} has been applied on {targetView.Name}." % Colorize.Orange);
            _onActivationInstruction?.Execute(effect);
            foreach (var triggerInstruction in _triggerInstructions)
            {
                var trigger = new BattleTrigger();
                var triggerContext 
                    = new TriggerActivationContext(triggerInstruction.Key, effect.BattleContext);

                trigger.Triggered += OnTriggered;
                trigger.Activate(triggerContext);

                void OnTriggered(ITriggerFiredInfo firedInfo)
                {
                    triggerInstruction.Value.Execute(effect);
                }
            }
        }

        public void OnDeactivation(BattleEffect effect)
        {
            var targetView = effect.BattleContext.EntitiesBank.GetViewByEntity(effect.EffectHolder);
            Debug.Log($"Effect {effect.EffectData.View.Name} has been removed from {targetView.Name}." % Colorize.Orange);
            _onDeactivationInstruction?.Execute(effect);
        }
    }
}
