using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "new Passive Ability", menuName = "OrderElimination/AbilitySystem/Passive Ability")]
    public class PassiveAbilityBuilder : SerializedScriptableObject
    {
        private int _cooldownTime = 0;

        [TitleGroup("Visuals", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 0), PropertyOrder(0)]
        [ShowInInspector, OdinSerialize]
        public string Name { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(1)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [ShowInInspector, OdinSerialize]
        public Sprite Icon { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(2)]
        [ShowInInspector, OdinSerialize, MultiLineProperty]
        public string Description { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(4)]
        [ShowInInspector, OdinSerialize]
        public bool HideInCharacterDiscription { get; private set; }

        [ValidateInput("@CooldownTime == 0", "Warining! Only first first-fired trigger's instruction will be executed.")]
        [TitleGroup("Game Rules"), PropertyOrder(1)]
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

        [TitleGroup("Functionality", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 4), PropertyOrder(0)]
        [ShowInInspector, OdinSerialize]
        public ITriggerInstruction[] TriggerInstructions { get; private set; }
    }
}
