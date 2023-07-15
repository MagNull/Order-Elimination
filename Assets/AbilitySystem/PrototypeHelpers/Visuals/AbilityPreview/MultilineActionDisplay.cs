using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class MultilineActionDisplay : MonoBehaviour
{
    [SerializeField] 
    private SpriteTextValueElement _parameterPrefab;
    [SerializeField]
    private Transform _parametersHolder;

    private ObjectPool<SpriteTextValueElement> _parametersPool;
    private int _insertIndex = 0;
    private readonly List<SpriteTextValueElement> _activeParameters = new();

    private void Awake()
    {
        _parametersPool = new(
            () => Instantiate(_parameterPrefab, _parametersHolder),
            p => p.gameObject.SetActive(true),
            p => p.gameObject.SetActive(false));
    }

    public void AddParameter(Sprite icon, string value)
    {
        AddParameter(icon, Color.white, value, Color.white);
    }

    public void AddParameter(Sprite icon, Color iconColor, string value)
    {
        AddParameter(icon, iconColor, value, Color.white);
    }

    public void AddParameter(Sprite icon, Color iconColor, string value, Color valueColor)
    {
        var parameter = CreateParameter(icon, value);
        parameter.SetIconColor(iconColor);
        parameter.SetValueColor(valueColor);
    }

    public void ClearParameters()
    {
        _insertIndex = 0;
        _activeParameters.ForEach(p => _parametersPool.Release(p));
        _activeParameters.Clear();
    }

    private SpriteTextValueElement CreateParameter(Sprite icon, string value)
    {
        var parameter = _parametersPool.Get();
        _activeParameters.Add(parameter);
        parameter.Icon = icon;
        parameter.Value = value;
        parameter.transform.SetSiblingIndex(_insertIndex);
        _insertIndex++;
        return parameter;
    }
}
