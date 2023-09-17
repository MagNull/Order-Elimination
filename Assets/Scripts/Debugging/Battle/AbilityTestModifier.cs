using OrderElimination;
using OrderElimination.AbilitySystem;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
using UnityEngine;

public class AbilityTestModifier : SerializedMonoBehaviour
{
    private BattleMapSelector _selector;

    [TitleGroup("Parent selection")]
    [ShowInInspector, OdinSerialize]
    private IBattleAction _parentAction;

    [TitleGroup("New instruction properties")]
    [ShowInInspector, OdinSerialize]
    private AbilityInstruction _instructionToAdd;

    [EnumToggleButtons]
    [ShowInInspector, OdinSerialize]
    private InstructionFollowType _followType;

    [ShowInInspector, OdinSerialize]
    private bool _copyParentTargetGroups;

    private void Start()
    {
        _selector = FindObjectOfType<BattleMapSelector>();
    }

    [ShowIf("@_selector != null && _selector.CurrentSelectedEntity != null")]
    [GUIColor("@Color.magenta")]
    [Button("Add Instructions to selected entity.")]
    private void ModifyCurrentEntityAbilities()
    {
        var currentEntity = _selector.CurrentSelectedEntity;
        if (currentEntity == null)
        {
            Logging.Log("Modification failed." % Colorize.Magenta);
            return;
        }
        var modifiedAbilities = 0;
        foreach (var ability in currentEntity.ActiveAbilities.Select(a => a.AbilityData))
        {
            if (ability.Execution.AppendInstructionRecursively(
                i => i.Action.GetType() == _parentAction.GetType(),
                _instructionToAdd,
                _copyParentTargetGroups,
                _followType))
                modifiedAbilities++;
        }
        Logging.Log($"Successfully modified {modifiedAbilities} abilities." % Colorize.Magenta);
    }
}
