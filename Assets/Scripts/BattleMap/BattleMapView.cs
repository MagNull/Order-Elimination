using System.Collections;
using System.Collections.Generic;
using OrderElimination.BattleMap;
using UnityEngine;

public class BattleMapView : MonoBehaviour
{
    [SerializeField]
    private BattleMap _battleMap;

    public BattleMap Map => _battleMap;

    public void OnEnable()
    {
        _battleMap.CellChanged += OnCellChanged;
    }

    public void OnDisable()
    {
        _battleMap.CellChanged -= OnCellChanged;
    }

    public void LightCellByDistance(int x, int y, int distance)
    {
        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                if (x + i >= 0 && x + i < _battleMap.Width && y + j >= 0 && y + j < _battleMap.Height)
                {
                    _battleMap.GetCell(x + i, y + j).Light();
                }
            }
        }
    }

    private void OnCellChanged(CellView cell)
    {
        // IBattleObject -> BattleObject не круто
        IBattleObject obj = cell.GetObject();
        if (obj is NullBattleObject)
            return;
        obj.GetView().transform.position = cell.transform.position;
        Debug.Log($"Cell {cell.name} changed");
    }
}