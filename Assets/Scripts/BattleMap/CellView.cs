using System;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;
    [SerializeField]
    private Renderer _renderer;
    private IBattleObject _object;

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
        _renderer.material.color = Color.red;
    }

    private void OnMouseDown()
    {
        CellClicked?.Invoke(this);
    }
}