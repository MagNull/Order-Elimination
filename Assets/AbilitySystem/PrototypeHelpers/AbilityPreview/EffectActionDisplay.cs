using TMPro;
using UnityEngine;

public class EffectActionDisplay : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [SerializeField]
    public TMP_Text _applyChanceText;

    public Sprite EffectIcon
    {
        get => _spriteRenderer.sprite;
        set => _spriteRenderer.sprite = value;
    }
    public string ApplyChance
    {
        get => _applyChanceText.text;
        set => _applyChanceText.text = value;
    }
}
