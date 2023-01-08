using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using OrderElimination.BattleMap;
using UnityEngine;

public class BattleMapView : MonoBehaviour
{
    public event Action<CellView> CellClicked;

    [SerializeField]
    private BattleMap _battleMap;
    [SerializeField]
    private float _moveDuration = 0.5f;

    private CellView[,] _cellViewGrid;

    private readonly List<CellView> _lightedCells = new();
    private bool _battleEnded = false;

    public BattleMap Map => _battleMap;

    public float MoveDuration => _moveDuration;

    public void OnEnable()
    {
        BattleSimulation.BattleEnded += OnBattleEnded;
        _battleMap.CellChanged += OnCellChanged;
    }

    public void OnDisable()
    {
        BattleSimulation.BattleEnded -= OnBattleEnded;
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

    public void LightCell(CellView cellView)
    {
        cellView.Light();
        _lightedCells.Add(cellView);
    }

    public void DelightCells()
    {
        foreach (var cell in _lightedCells)
        {
            cell?.Delight();
        }
        _lightedCells.Clear();
    }

    private void OnBattleEnded(BattleOutcome obj)
    {
        _battleEnded = true;
    }

    private void OnCellClicked(CellView cellView)
    {
        if(_battleEnded)
            return;
        CellClicked?.Invoke(cellView);
    }

    private void OnCellChanged(Cell cell)
    {
        var obj = cell.GetObject();
        if (obj is NullBattleObject)
            return;
        obj.View.transform.DOMove(GetCell(obj).transform.position, _moveDuration) ;
    }
}