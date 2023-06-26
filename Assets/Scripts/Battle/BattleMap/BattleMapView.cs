using System;
using System.Collections.Generic;
using OrderElimination.Infrastructure;
using UnityEngine;

//TODO: Optimize some methods (find cell example)
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

    /// <summary>
    /// Interpolates game position between MapView borders.
    /// </summary>
    /// <param name="gamePosition"></param>
    /// <returns>Returns real world position.</returns>
    public Vector2 GameToWorldPosition(Vector2 gamePosition)
    {
        var gameStartPoint = new Vector2Int(Map.CellRangeBorders.xMin, Map.CellRangeBorders.yMin);
        var gameEndPoint = new Vector2Int(Map.CellRangeBorders.xMax, Map.CellRangeBorders.yMax);
        var offset = GetCell(0, 0).transform.position;
        var viewXBasis = GetCell(gameEndPoint.x, 0).transform.position;
        var viewYBasis = GetCell(0, gameEndPoint.y).transform.position;

        var xUnLerp = CellMath.InverseLerpUnclamped(gameStartPoint.x, gameEndPoint.x, gamePosition.x);
        var yUnLerp = CellMath.InverseLerpUnclamped(gameStartPoint.y, gameEndPoint.y, gamePosition.y);

        return offset + (viewXBasis - offset) * xUnLerp + (viewYBasis - offset) * yUnLerp;
    }

    public void OnDisable()
    {
        if (_cellViewGrid == null) return;
        foreach (var cellView in _cellViewGrid)
        {
            cellView.CellClicked -= OnCellClicked;
        }
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

    public void DelightCells()
    {
        foreach (var cell in _lightedCells)
        {
            cell?.Delight();
        }

        _lightedCells.Clear();
    }

    public void HighlightCell(int x, int y, Color color)
    {
        CellView cell = _cellViewGrid[x, y];
        cell.Highlight(color);
        _lightedCells.Add(cell);
    }

    private void OnCellClicked(CellView cellView)
    {
        if (_battleEnded)
            return;
        CellClicked?.Invoke(cellView);
    }
}