using System;
using OrderElimination.BattleMap;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;
    [SerializeField]
    private Renderer _renderer;
    [SerializeField]
    private Color _selectedColor;
    private Color _deselectColor;
    private IBattleObject _object;
    private Color _basicColor;

    public void Start()
    {
        _basicColor = _renderer == null ? Color.white : _renderer.material.color;
    }

    public IBattleObject GetObject()
    {
        return _object;
    }

    public void SetObject(IBattleObject obj)
    {
        _object = obj;
    }

    public void Light()
    {
        _renderer.material.color = _object.Side switch
        {
            BattleObjectSide.None => Color.red,
            BattleObjectSide.Enemy => Color.magenta,
            _ => Color.blue
        };
    }

    public void Delight()
    {
        Debug.Log("Light is turned out");
        _renderer.material.color = _basicColor;
    }

    public void Select()
    {
        _deselectColor = _renderer.material.color;
        _renderer.material.color = _selectedColor;
    }

    public void Deselect() => _renderer.material.color = _deselectColor;

    private void OnMouseDown()
    {
        CellClicked?.Invoke(this);
    }
}