using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid
{
    private CellView[,] _cellViewGrid;
    private Cell[,] _cellModelGrid;

    public CellView[,] View => _cellViewGrid;
    public Cell[,] Model => _cellModelGrid;

    public CellGrid(CellView[,] view, Cell[,] model)
    {
        _cellViewGrid = view;
        _cellModelGrid = model;
    }
}
