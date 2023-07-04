using OrderElimination.AbilitySystem;
using TMPro;
using UnityEngine;

public class HealActionDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _healText;

    public string HealValue
    {
        get => _healText.text;
        set => _healText.text = value;
    }
}
