using OrderElimination.AbilitySystem.UI;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    //[Title("Ability Template", "Active Ability", TitleAlignment = TitleAlignments.Centered)]
    [CreateAssetMenu(fileName = "new Active Ability", menuName = "OrderElimination/AbilitySystem/Active Ability")]
    public class ActiveAbilityBuilder : SerializedScriptableObject
    {
        private int _cooldownTime;
        private int _necessaryTargets;
        private int _optionalTargets;

        [TabGroup("MainTabs", "Visuals", Order = -2), PropertyOrder(-100)]
        [HorizontalGroup("MainTabs/Visuals/NameIconDescr", Width = 0.3f)]
        [VerticalGroup("MainTabs/Visuals/NameIconDescr/Left")]
        [HideLabel, Title("Name", HorizontalLine = false)]
        [ShowInInspector, OdinSerialize]
        public string Name { get; private set; } = "";

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(1)]
        [HorizontalGroup("MainTabs/Visuals/NameIconDescr")]
        [VerticalGroup("MainTabs/Visuals/NameIconDescr/Left")]
        [PreviewField(70, Alignment = ObjectFieldAlignment.Left)]
        [HideLabel]
        [ShowInInspector, OdinSerialize]
        public Sprite Icon { get; private set; }

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(2)]
        [HorizontalGroup("MainTabs/Visuals/NameIconDescr", PaddingLeft = 5)]
        [VerticalGroup("MainTabs/Visuals/NameIconDescr/Right")]
        [HideLabel, Title("Description", HorizontalLine = false)]
        [PropertySpace(SpaceBefore = 0, SpaceAfter = 5), MultiLineProperty(5)]
        [ShowInInspector, OdinSerialize]
        public string Description { get; private set; } = "";

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(-99)]
        [TitleGroup("MainTabs/Visuals/Battle Visuals", BoldTitle = true)]
        [DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Highlight color")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<int, Color> _cellGroupsHighlightColors = new();
        public IReadOnlyDictionary<int, Color> CellGroupsHighlightColors => _cellGroupsHighlightColors;

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(4)]
        [TitleGroup("MainTabs/Visuals/Battle Visuals")]
        [ShowInInspector, OdinSerialize]
        public bool ShowCrosshairWhenTargeting { get; private set; } = true;

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(5)]
        [TitleGroup("MainTabs/Visuals/Battle Visuals")]
        [ShowInInspector, OdinSerialize]
        public bool ShowTrajectoryWhenTargeting { get; private set; } = false;

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(6)]
        [TitleGroup("MainTabs/Visuals/Battle Visuals")]
        [ShowInInspector, OdinSerialize]
        public bool ShowPatternRange { get; private set; } = true;

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(7)]
        [TitleGroup("MainTabs/Visuals/Description Visuals", BoldTitle = true)]
        [ShowInInspector, OdinSerialize]
        public bool HideInCharacterDescription { get; private set; }

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(8)]
        [TitleGroup("MainTabs/Visuals/Description Visuals")]
        [ShowInInspector, OdinSerialize]
        public bool HideAutoParameters { get; private set; } = false;

        [TabGroup("MainTabs", "Visuals"), PropertyOrder(10)]
        [TitleGroup("MainTabs/Visuals/Description Visuals")]
        [ListDrawerSettings(ShowFoldout = false)]
        [TableList(AlwaysExpanded = true)]
        //[DictionaryDrawerSettings(KeyLabel = "Parameter Name", ValueLabel = "Value")]
        [ShowInInspector, OdinSerialize]
        private List<CustomViewParameter> _customParameters = new();
        public IReadOnlyList<CustomViewParameter> CustomParameters => _customParameters;

        [TabGroup("MainTabs", "Game Rules", Order = 1), PropertyOrder(-98)]
        [DictionaryDrawerSettings(KeyLabel = "Energy Point", ValueLabel = "Amount")]
        [ShowInInspector, OdinSerialize]
        private Dictionary<EnergyPoint, int> _usageCost = new();
        public IReadOnlyDictionary<EnergyPoint, int> UsageCost => _usageCost;

        [TabGroup("MainTabs", "Game Rules"), PropertyOrder(1)]
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

        [TabGroup("MainTabs", "Game Rules"), PropertyOrder(3)]
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] AvailabilityConditions = new ICommonCondition[0];

        [TabGroup("MainTabs", "Targeting System", Order = 2), PropertyOrder(-97)]
        [BoxGroup("MainTabs/Targeting System/System Settings", ShowLabel = false)]
        [PropertySpace(5)]
        [ShowInInspector, OdinSerialize]
        public TargetingSystemType TargetingSystem { get; private set; }

        [TabGroup("MainTabs", "Targeting System"), PropertyOrder(5)]
        [BoxGroup("MainTabs/Targeting System/System Settings", ShowLabel = false)]
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

        [TabGroup("MainTabs", "Targeting System"), PropertyOrder(6)]
        [BoxGroup("MainTabs/Targeting System/System Settings", ShowLabel = false)]
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

        [TabGroup("MainTabs", "Targeting System"), PropertyOrder(7)]
        [PropertySpace(5)]
        [ShowInInspector, OdinSerialize]
        [ShowIf("@TargetingSystem == TargetingSystemType.MultiTarget || TargetingSystem == TargetingSystemType.SingleTarget")]
        public ICellCondition[] TargetCellConditions = new ICellCondition[0];

        [TabGroup("MainTabs", "Targeting System"), PropertyOrder(8)]
        [BoxGroup("MainTabs/Targeting System/Distributor", ShowLabel = false)]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [ValidateInput("@" + nameof(CellGroupsDistributor) + " != null", "Distributor is not assigned!")]
        [PropertySpace(5)]
        [ShowInInspector, OdinSerialize]
        public ICellGroupsDistributor CellGroupsDistributor { get; private set; }

        [TabGroup("MainTabs", "Functionality", Order = 4), PropertyOrder(-96)]
        [OnInspectorInit("@$property.State.Expanded = true")]
        [ShowInInspector, OdinSerialize]
        public AbilityInstruction[] AbilityInstructions = new AbilityInstruction[0];
    }

    public enum TargetingSystemType
    {
        NoTarget,
        SingleTarget,
        MultiTarget
    }
}
