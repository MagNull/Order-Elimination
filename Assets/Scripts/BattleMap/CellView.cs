using System;
using OrderElimination.BattleMap;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;
    [SerializeField]
    private Renderer _renderer;
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
        Debug.Log("Light");
        _renderer.material.color = _object.Side == BattleObjectSide.None
            ? Color.red 
            : (_object.Side == BattleObjectSide.Enemy 
                ? Color.magenta
                : Color.blue);
    }

    public void Delight()
    {
        Debug.Log("Light is turned out");
        _renderer.material.color = _basicColor;
    }

    private void OnMouseDown()
    {
        CellClicked?.Invoke(this);
    }
}