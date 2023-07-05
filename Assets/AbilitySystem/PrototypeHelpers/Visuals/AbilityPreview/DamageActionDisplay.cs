using OrderElimination.AbilitySystem;
using TMPro;
using UnityEngine;

public class DamageActionDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _damageText;
    [SerializeField]
    private TMP_Text _accuracyText;

    public string DamageValue
    {
        get => _damageText.text;
        set => _damageText.text = value;
    }

    public string AccuracyValue
    {
        get => _accuracyText.text;
        set => _accuracyText.text = value;
    }
}
