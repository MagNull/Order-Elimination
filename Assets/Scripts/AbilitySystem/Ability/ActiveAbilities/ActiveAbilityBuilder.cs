using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "new Active Ability", menuName = "OrderElimination/AbilitySystem/Active Ability")]
    public class ActiveAbilityBuilder : SerializedScriptableObject
    {
        private int _cooldownTime;
        private int _necessaryTargets;
        private int _optionalTargets;

        [TabGroup("Visuals", Order = 0), PropertyOrder(-99)]
        [ShowInInspector, OdinSerialize]
        public string Name { get; private set; } = "";

        [TabGroup("Visuals"), PropertyOrder(1)]
        [PreviewField(Alignment = ObjectFieldAlignment.Left)]
        [ShowInInspector, OdinSerialize]
        public Sprite Icon { get; private set; }

        [TabGroup("Visuals"), PropertyOrder(2)]
        [ShowInInspector, OdinSerialize, MultiLineProperty]
        public string Description { get; private set; } = "";

        //[TitleGroup("Visuals"), PropertyOrder(2.5f)]
        //[PreviewField(Alignment = ObjectFieldAlignment.Left)]
        //[ShowInInspector, OdinSerialize]
        //public VideoClip PreviewVideo { get; private set; }

        [TabGroup("Visuals"), PropertyOrder(3)]
        [ShowInInspector, OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Highlight color")]
        private Dictionary<int, Color> _cellGroupsHighlightColors = new();
        public IReadOnlyDictionary<int, Color> CellGroupsHighlightColors => _cellGroupsHighlightColors;

        [TabGroup("Visuals"), PropertyOrder(4)]
        [ShowInInspector, OdinSerialize]
        public bool ShowCrosshairWhenTargeting { get; private set; } = true;

        [TabGroup("Visuals"), PropertyOrder(5)]
        [ShowInInspector, OdinSerialize]
        public bool ShowTrajectoryWhenTargeting { get; private set; } = false;

        [TabGroup("Visuals"), PropertyOrder(10)]
        [ShowInInspector, OdinSerialize]
        public bool HideInCharacterDescription { get; private set; }

        [TabGroup("Game Rules", Order = 1), PropertyOrder(-98)]
        [DictionaryDrawerSettings(KeyLabel = "Energy Point", ValueLabel = "Amount")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<EnergyPoint, int> _usageCost = new();
        public IReadOnlyDictionary<EnergyPoint, int> UsageCost => _usageCost;

        [TabGroup("Game Rules"), PropertyOrder(1)]
        [PropertySpace(SpaceBefore = 10, SpaceAfter = 10)]
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

        [TabGroup("Game Rules"), PropertyOrder(3)]
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] AvailabilityConditions = new ICommonCondition[0];

        [TabGroup("Targeting System", Order = 2), PropertyOrder(-97)]
        [PropertySpace(5)]
        [ShowInInspector, OdinSerialize]
        public TargetingSystemType TargetingSystem { get; private set; }

        [TabGroup("Targeting System"), PropertyOrder(5)]
        [ShowInInspector, OdinSerialize]
        [ShowIf("@TargetingSystem == TargetingSystemType.MultiTarget")]
        public int NecessaryTargets
        {
            get => _necessaryTargets;
            set
            {
                if (value < 0) value = 0;
                _necessaryTargets = value;
            }
        }

        [TabGroup("Targeting System"), PropertyOrder(6)]
        [ShowInInspector, OdinSerialize]
        [ShowIf("@TargetingSystem == TargetingSystemType.MultiTarget")]
        public int OptionalTargets
        {
            get => _optionalTargets;
            set
            {
                if (value < 0) value = 0;
                _optionalTargets = value;
            }
        }

        [TabGroup("Targeting System"), PropertyOrder(7)]
        [PropertySpace(5)]
        [ShowInInspector, OdinSerialize]
        [ShowIf("@TargetingSystem == TargetingSystemType.MultiTarget || TargetingSystem == TargetingSystemType.SingleTarget")]
        public ICellCondition[] TargetCellConditions = new ICellCondition[0];

        [TabGroup("Targeting System"), PropertyOrder(8)]
        [PropertySpace(5)]
        [ShowInInspector, OdinSerialize]
        public ICellGroupsDistributor CellGroupsDistributor { get; private set; }

        [TabGroup("Functionality", Order = 4), PropertyOrder(-96)]
        [ShowInInspector, OdinSerialize]
        public AbilityInstruction[] AbilityInstructions;
    }

    public enum TargetingSystemType
    {
        NoTarget,
        SingleTarget,
        MultiTarget
    }
}
