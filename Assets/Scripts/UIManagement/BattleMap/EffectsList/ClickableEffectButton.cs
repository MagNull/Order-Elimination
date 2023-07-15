using System;
using TMPro;
using UIManagement.Elements;
using UnityEngine;
using UnityEngine.UI;

public class ClickableEffectButton : MonoBehaviour
{
    [SerializeField]
    private Image _icon;
    [SerializeField]
    private TextMeshProUGUI _stackNumbersText;
    [SerializeField]
    private HoldableButton _button;

    public Image IconImage => _icon;
    public TextMeshProUGUI StackNumbersText => _stackNumbersText;

    public event Action<ClickableEffectButton> Clicked;

    private void Awake()
    {
        _button.Clicked -= OnButtonClicked;
        _button.Clicked += OnButtonClicked;
    }

    private void OnButtonClicked(HoldableButton button)
    {
        Clicked?.Invoke(this);
    }
}
