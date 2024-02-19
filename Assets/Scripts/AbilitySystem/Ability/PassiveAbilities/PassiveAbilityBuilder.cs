using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //[Title("Ability Template", "Passive Ability", TitleAlignment = TitleAlignments.Centered)]
    [CreateAssetMenu(fileName = "new Passive Ability", menuName = "OrderElimination/AbilitySystem/Passive Ability")]
    public class PassiveAbilityBuilder : SerializedScriptableObject
    {
        private int _cooldownTime = 0;

        [TitleGroup("Visuals", Alignment = TitleAlignments.Centered, Order = 0), PropertyOrder(0)]
        [HorizontalGroup("Visuals/NameIconDescr", Width = 0.3f)]
        [VerticalGroup("Visuals/NameIconDescr/Left")]
        [HideLabel, Title("Name", HorizontalLine = false)]
        [ShowInInspector, OdinSerialize]
        public string Name { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(1)]
        [HorizontalGroup("Visuals/NameIconDescr")]
        [VerticalGroup("Visuals/NameIconDescr/Left")]
        [PreviewField(70, Alignment = ObjectFieldAlignment.Left)]
        [HideLabel]
        [ShowInInspector, OdinSerialize]
        public Sprite Icon { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(2)]
        [HorizontalGroup("Visuals/NameIconDescr")]
        [VerticalGroup("Visuals/NameIconDescr/Right")]
        [HideLabel, Title("Description", HorizontalLine = false)]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5), MultiLineProperty(5)]
        [ShowInInspector, OdinSerialize]
        public string Description { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(4)]
        [ShowInInspector, OdinSerialize]
        public bool HideInCharacterDescription { get; private set; }

        [ValidateInput("@CooldownTime == 0", "Warining! Only first first-fired trigger's instruction will be executed.")]
        [TitleGroup("Game Rules", Alignment = TitleAlignments.Centered), PropertyOrder(1)]
        [ShowInInspector, OdinSerialize]
        public int CooldownTime
        {
            get => _cooldownTime;
            private set
            {
                if (value < 0) value = 0;
                _cooldownTime = value;
            }
        }

        [TitleGroup("Functionality", Alignment = TitleAlignments.Centered, Order = 4), PropertyOrder(0)]
        [PropertyTooltip("Actions performed with Owner of Passive Ability on activation."
            + "\nUndone on deactivation if possible.")]
        [ShowInInspector, OdinSerialize]
        public IUndoableBattleAction[] ActionsOnActivation { get; private set; } = new IUndoableBattleAction[0];

        [TitleGroup("Functionality", Alignment = TitleAlignments.Centered, Order = 4), PropertyOrder(0)]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector, OdinSerialize]
        public ITriggerInstruction[] TriggerInstructions { get; private set; } = new ITriggerInstruction[0];
    }
}
