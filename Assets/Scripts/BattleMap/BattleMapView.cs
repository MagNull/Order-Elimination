using System;
using System.Collections;
using System.Collections.Generic;
using OrderElimination.BattleMap;
using UnityEngine;

public class BattleMapView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private BattleMap _battleMap;

    private CellView[,] _cellViewGrid;

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

    public void Init(CellView[,] viewGrid)
    {
        _cellViewGrid = viewGrid;

        foreach (var cellView in _cellViewGrid)
        {
            cellView.CellClicked += OnCellClicked;
        }
    }

    public CellView GetCell(int x, int y)
    {
        return _cellViewGrid[x, y];
    }

    public CellView GetCell(IBattleObject battleObject)
    {
        for (var i = 0; i < _cellViewGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellViewGrid.GetLength(1); j++)
            {
                if (_cellViewGrid[i, j].Model.GetObject() == battleObject)
                {
                    return _cellViewGrid[i, j];
                }
            }
        }

        throw new ArgumentException("BattleObject not found");
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
                    LightCell(deltedX, deltedY);
                }
            }
        }
    }

    public void LightCell(int x, int y)
    {
        CellView cell = _cellViewGrid[x, y];
        cell.Light();
        _lightedCells.Add(cell);
    }

    public void DelightCells()
    {
        foreach (var cell in _lightedCells)
        {
            cell.Delight();
        }
    }

    private void OnCellClicked(CellView cellView) => CellClicked?.Invoke(cellView);

    private void OnCellChanged(CellModel cellModel)
    {
        IBattleObject obj = cellModel.GetObject();
        if (obj is NullBattleObject)
            return;
        obj.GetView().transform.position = GetCell(obj).transform.position;
    }
}