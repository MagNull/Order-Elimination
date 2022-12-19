using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CharacterAbility;

public class AbilityChanceHandler : MonoBehaviour
{
    [SerializeField] private ProcChanceWindow _chanceWindow;

    public void OnEnable()
    {
        AbilityView.TargetSelected += ShowAbilityProcChance;
        AbilityView.TargetDeselected += HideAbilityProcChance;
    }

    public void OnDisable()
    {
        AbilityView.TargetSelected -= ShowAbilityProcChance;
        AbilityView.TargetDeselected -= HideAbilityProcChance;
    }

    private void ShowAbilityProcChance(float probability, GameObject targetView)
    {
        HideAbilityProcChance();
        _chanceWindow.Show(probability, targetView);
    }

    private void HideAbilityProcChance()
    {
        _chanceWindow.Hide();
    }
}
