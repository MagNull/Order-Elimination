using System.Collections;
using System.Collections.Generic;
using OrderElimination.BattleMap;
using UnityEngine;

public class BattleMapView : MonoBehaviour
{
    [SerializeField]
    private BattleMap _battleMap;

    private List<CellView> _lightedCells = new List<CellView>();

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
                int deltedX = x + i;
                int deltedY = y + j;

                if (deltedX >= 0 && deltedX < _battleMap.Width && deltedY >= 0 && deltedY < _battleMap.Height)
                {
                    CellView cell = _battleMap.GetCell(deltedX, deltedY);
                    cell.Light();
                    _lightedCells.Add(cell);
                }
            }
        }
    }

    public void DelightCells()
    {
        foreach (var cell in _lightedCells)
        {
            cell.Delight();
        }
    }

    private void OnCellChanged(CellView cell)
    {
        IBattleObject obj = cell.GetObject();
        if (obj is NullBattleObject)
            return;
        obj.GetView().transform.position = cell.transform.position;
    }
}