using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using OrderElimination.BM;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
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

    public void OnEnable()
    {
        _battleMap.CellChangedOld += OnCellChanged;
    }

    public void OnDisable()
    {
        BattleSimulation.BattleEnded -= OnBattleEnded;
        _battleMap.CellChangedOld -= OnCellChanged;
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

    public CellView GetCell(IBattleObject battleObject)
    {
        for (var i = 0; i < _cellViewGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellViewGrid.GetLength(1); j++)
            {
                if (_cellViewGrid[i, j].Model.Objects.Contains(battleObject))
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
                    HighlightCell(deltedX, deltedY, Color.white);
                }
            }
        }
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

    private void OnCellChanged(Cell cell, bool tween)
    {
        var objs = cell.Objects;
        if (objs.Count <= 1)
            return;
        foreach (var battleObject in objs)
        {
            if (battleObject is NullBattleObject)
                continue;
            if (tween)
            {
                Debug.Log("Move");
                battleObject.View.GameObject.transform.DOMove(GetCell(battleObject).transform.position, _moveDuration);
            }
            else
            {
                Debug.Log("Spawn");
                battleObject.View.GameObject.transform.position = GetCell(battleObject).transform.position;
            }
        }
    }
}