using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "AbilityBuilderData", menuName = "AbilitySystem/AbilityBuilderData")]
    public class AbilityBuilderData : SerializedScriptableObject
    {
        [TitleGroup("Visuals", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 0), PropertyOrder(0)]
        [ShowInInspector, OdinSerialize]
        public string Name { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(1)]
        [ShowInInspector, OdinSerialize, PreviewField(Alignment = ObjectFieldAlignment.Left)]
        public Sprite Icon { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(2)]
        [ShowInInspector, OdinSerialize, MultiLineProperty]
        public string Description { get; private set; }

        [TitleGroup("Visuals"), PropertyOrder(3)]
        [ShowInInspector, OdinSerialize, DictionaryDrawerSettings(KeyLabel = "Group", ValueLabel = "Highlight color")]
        private Dictionary<int, Color> _cellGroupsHighlightColors = new();
        public IReadOnlyDictionary<int, Color> CellGroupsHighlightColors => _cellGroupsHighlightColors;

        [TitleGroup("Game Rules", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 1), PropertyOrder(0)]
        [ShowInInspector, OdinSerialize]
        private Dictionary<ActionPoint, int> _usageCost = new();
        public IReadOnlyDictionary<ActionPoint, int> UsageCost => _usageCost;

        [TitleGroup("Game Rules"), PropertyOrder(1)]
        [ShowInInspector, OdinSerialize]
        public int CooldownTime { get; private set; }

        [TitleGroup("Game Rules"), PropertyOrder(2)]
        [ShowInInspector, OdinSerialize]
        public int UnlocksAtRound { get; private set; }

        [TitleGroup("Game Rules"), PropertyOrder(3)]
        [ShowInInspector, OdinSerialize]
        public ICommonCondition[] AvailabilityConditions = new ICommonCondition[0];

        [TitleGroup("Targeting System", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 2), PropertyOrder(4)]
        [ShowInInspector, OdinSerialize]
        public TargetingSystemType TargetingSystem { get; private set; }

        //FOR MULTITARGET ABILITIES
        [TitleGroup("Targeting System"), PropertyOrder(5)]
        [ShowInInspector, OdinSerialize]
        [ShowIf("@TargetingSystem == TargetingSystemType.MultiTarget || TargetingSystem == TargetingSystemType.SingleTarget")]
        public ICellCondition[] TargetCellConditions = new ICellCondition[0];

        [TitleGroup("Targeting System"), PropertyOrder(6)]
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
        private int _necessaryTargets;

        [TitleGroup("Targeting System"), PropertyOrder(7)]
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
        private int _optionalTargets;

        //FOR MULTITARGET ABILITIES

        [TitleGroup("Targeting System"), PropertyOrder(8)]
        [ShowInInspector, OdinSerialize]
        [ValidateInput(nameof(ValidateCellPattern)), Tooltip("Defines how target groups for execution calculated.")]
        public CellGroupDistributionPattern DistributionPattern { get; private set; }

        [TitleGroup("Functionality", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 4), PropertyOrder(0)]
        [ShowInInspector, OdinSerialize]
        public ActionInstruction[] AbilityInstructions;

        //private const float TitleSpacing = 50;
        private bool ValidateCellPattern(CellGroupDistributionPattern pattern)
        {
            if (TargetingSystem == TargetingSystemType.NoTarget)
            {
                return pattern is CasterRelativePattern;
            }
            else if (TargetingSystem == TargetingSystemType.MultiTarget || TargetingSystem == TargetingSystemType.SingleTarget)
            {
                return pattern is TargetRelativePattern or CasterToTargetRelativePattern;
            }
            return false;
        }
    }

    public enum TargetingSystemType
    {
        NoTarget,
        SingleTarget,
        MultiTarget
    }
}
