using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace OrderElimination.AbilitySystem
{
    [CreateAssetMenu(fileName = "new Active Ability", menuName = "AbilitySystem/Ability Ability")]
    public class ActiveAbilityBuilder : SerializedScriptableObject
    {
        private int _cooldownTime;
        private int _necessaryTargets;
        private int _optionalTargets;


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

        //[TitleGroup("Visuals"), PropertyOrder(2.5f)]
        //[PreviewField(Alignment = ObjectFieldAlignment.Left)]
        //[ShowInInspector, OdinSerialize]
        //public VideoClip PreviewVideo { get; private set; }

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
        public int CooldownTime
        {
            get => _cooldownTime;
            private set
            {
                if (value < 0) value = 0;
                _cooldownTime = value;
            }
        }

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

        //FOR MULTITARGET ABILITIES

        [TitleGroup("Targeting System"), PropertyOrder(8)]
        [ShowInInspector, OdinSerialize]
        [ValidateInput(nameof(ValidateCellPattern))]
        public CellGroupDistributionPattern DistributionPattern { get; private set; }

        [TitleGroup("Functionality", BoldTitle = true, Alignment = TitleAlignments.Centered, Order = 4), PropertyOrder(0)]
        [ShowInInspector, OdinSerialize]
        public AbilityInstruction[] AbilityInstructions;

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
