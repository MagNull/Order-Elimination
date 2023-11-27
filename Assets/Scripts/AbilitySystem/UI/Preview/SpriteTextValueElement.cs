using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class SpriteTextValueElement : MonoBehaviour
{
    [Title("Components")]
    [SerializeField]
    private SpriteRenderer _iconComponent;
    [SerializeField]
    private TextMeshPro _textComponent;
    [SerializeField]
    private TextMeshPro _valueComponent;

    [Title("Settings")]
    [ShowInInspector]
    public bool HasIcon
    {
        get => _iconComponent == null ? false : _iconComponent.gameObject.activeSelf;
        set
        {
            if (_iconComponent != null)
                _iconComponent.gameObject.SetActive(value);
        }
    }
    [ShowInInspector]
    public bool HasText
    {
        get => _textComponent == null ? false : _textComponent.gameObject.activeSelf;
        set
        {
            if (_textComponent != null)
                _textComponent.gameObject.SetActive(value);
        }
    }
    [ShowInInspector]
    public bool HasValue
    {
        get => _valueComponent == null ? false : _valueComponent.gameObject.activeSelf;
        set
        {
            if (_valueComponent != null)
                _valueComponent.gameObject.SetActive(value);
        }
    }
    [ShowInInspector, PreviewField(Alignment = ObjectFieldAlignment.Left)]
    public Sprite Icon
    {
        get => _iconComponent == null ? null : _iconComponent.sprite;
        set
        {
            if (_iconComponent != null)
                _iconComponent.sprite = value;
        }
    }
    [ShowInInspector]
    public string Text
    {
        get => _textComponent == null ? null : _textComponent.text;
        set
        {
            if (_textComponent != null)
                _textComponent.text = value;
        }
    }
    [ShowInInspector]
    public string Value
    {
        get => _valueComponent == null ? null : _valueComponent.text;
        set
        {
            if (_valueComponent != null)
                _valueComponent.text = value;
        }
    }

    public bool SetIconColor(Color color)
    {
        if (_iconComponent != null)
        {
            _iconComponent.color = color;
            return true;
        }
        return false;
    }

    public bool SetTextColor(Color color)
    {
        if (_textComponent != null)
        {
            _textComponent.color = color;
            return true;
        }
        return false;
    }

    public bool SetValueColor(Color color)
    {
        if (_valueComponent != null)
        {
            _valueComponent.color = color;
            return true;
        }
        return false;
    }
}
