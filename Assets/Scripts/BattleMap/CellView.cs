using System;
using OrderElimination.BattleMap;
using UnityEngine;

public class CellView : MonoBehaviour
{
    public event Action<CellView> CellClicked;
    private IBattleObject _object;

    public IBattleObject GetObject()
    {
        return _object;
    }

    public void SetObject(IBattleObject obj)
    {
        _object = obj;
    }

    private void OnMouseDown()
    {
        CellClicked?.Invoke(this);
    }
}