using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using OrderElimination;
using OrderElimination.BattleMap;

public class BattleMap : MonoBehaviour
{
    public event Action<CellView> CellSelected;
    public event Action<CellView> CellChanged;

    [SerializeField]
    private int _width;
    [SerializeField]
    private int _height;
    [SerializeField]
    private CellGridGenerator _generator;
    //[SerializeField]
    //private BattleCharacterFactory _characterFactory;
    //[SerializeField]
    //private Character _characterInfo;

    private CellView[,] _cellGrid;

    public int Width => _width;

    public int Height => _height;

    public void Init()
    {
        // Создание игрового поля
        _cellGrid = _generator.GenerateGrid(_width, _height);
        foreach (var cellView in _cellGrid)
        {
            cellView.CellClicked += OnCellClicked;
        }
    }

    public CellView GetCell(int x, int y)
    {
        return _cellGrid[x, y];
    }

    public void SetCell(int x, int y, IBattleObject obj)
    {
        // Ставим клетку в точку (x,y)
        // Вызываем срабатывание ивента
        _cellGrid[x, y].SetObject(obj);
        CellChanged?.Invoke(_cellGrid[x, y]);
    }

    public void MoveTo(IBattleObject obj, int x, int y)
    {
        Vector2Int objCrd = GetCoordinate(obj);
        _cellGrid[objCrd.x, objCrd.y].SetObject(new NullBattleObject());
        SetCell(x, y, obj);
    }

    public void OnCellClicked(CellView cellView)
    {
        CellSelected?.Invoke(cellView);
    }

    public Vector2Int GetCoordinate(IBattleObject obj)
    {
        for (var i = 0; i < _cellGrid.GetLength(0); i++)
        {
            for (var j = 0; j < _cellGrid.GetLength(1); j++)
            {
                if (_cellGrid[i, j].GetObject() == obj)
                {
                    return new Vector2Int(i, j);
                }
            }
        }

        Debug.Log("Объект не найден на поле!");
        return new Vector2Int(-1, -1);
    }

    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius, BattleObjectSide side)
    {
        return GetObjectsInRadius(obj, radius, battleObject => 
            battleObject is not NullBattleObject && battleObject.Side == side);
    }
    
    public IList<IBattleObject> GetBattleObjectsInRadius(IBattleObject obj, int radius)
    {
        return GetObjectsInRadius(obj, radius, battleObject => 
            battleObject is not NullBattleObject);
    }

    public IList<IBattleObject> GetEmptyObjectsInRadius(IBattleObject obj, int radius)
    {
        return GetObjectsInRadius(obj, radius, battleObject => battleObject is NullBattleObject);
    }

    private IList<IBattleObject> GetObjectsInRadius(IBattleObject obj, int radius, Predicate<IBattleObject> predicate)
    {
        Vector2Int objCrd = GetCoordinate(obj);
        List<IBattleObject> objects = new List<IBattleObject>();
        for (var i = objCrd.x - radius; i <= objCrd.x + radius; i++)
        {
            for (var j = objCrd.y - radius; j <= objCrd.y + radius; j++)
            {
                if (i >= 0 && i < _cellGrid.GetLength(0) && j >= 0 && j < _cellGrid.GetLength(1))
                {
                    if (predicate(_cellGrid[i, j].GetObject()))
                    {
                        objects.Add(_cellGrid[i, j].GetObject());
                    }
                }
            }
        }

        objects.Remove(obj);
        return objects;
    }
}