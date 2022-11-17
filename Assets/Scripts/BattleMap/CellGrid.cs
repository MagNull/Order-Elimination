using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    private CellView[,] _cellViewGrid;
    private CellModel[,] _cellModelGrid;

    public CellView[,] View => _cellViewGrid;
    public CellModel[,] Model => _cellModelGrid;

    public CellGrid(CellView[,] view, CellModel[,] model)
    {
        _cellViewGrid = view;
        _cellModelGrid = model;
    }
}
