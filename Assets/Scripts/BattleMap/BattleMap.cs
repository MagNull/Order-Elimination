using UnityEngine;
using System;
using System.Collections.Generic;

public class BattleMap : MonoBehaviour
{
    public event Action<CellView> CellChanged;

    [SerializeField]
    CellGridGenerator _generator;
    [SerializeField]
    PlayerSpawner _spawner;

    private CellView[,] _cellGrid;

    public void Start()
    {
        // �������� �������� ����
        _cellGrid = _generator.GenerateGrid();

        // ����� ��������� ��������� �� ����
        IBattleObject obj = _spawner.Spawn(_cellGrid[0, 0]);
        // ������������ ��������� � ������
        SetCell(0, 0, obj);

        // ���� ����������� �������
        MoveTo(obj, 1, 1);
    }

    public CellView GetCell(int x, int y)
    {
        return _cellGrid[x, y];
    }

    public void SetCell(int x, int y, IBattleObject obj)
    {
        // ������ ������ � ����� (x,y)
        // �������� ������������ ������
        _cellGrid[x, y].SetObject(obj);
        CellChanged?.Invoke(_cellGrid[x, y]);
    }

    public void MoveTo(IBattleObject obj, int x, int y)
    {
        Vector2Int objCrd = GetCoordinate(obj);
        // �� null, � null-object?
        _cellGrid[objCrd.x, objCrd.y].SetObject(null);
        SetCell(x, y, obj);
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

        Debug.LogError("������ �� ������ �� ����!");
        return new Vector2Int(-1, -1);
    }

    public IBattleObject[] GetObjectsInRadius(IBattleObject obj, int radius)
    {
        Vector2Int objCrd = GetCoordinate(obj);
        List<IBattleObject> objects = new List<IBattleObject>();
        for (var i = objCrd.x - radius; i <= objCrd.x + radius; i++)
        {
            for (var j = objCrd.y - radius; j <= objCrd.y + radius; j++)
            {
                if (i >= 0 && i < _cellGrid.GetLength(0) && j >= 0 && j < _cellGrid.GetLength(1))
                {
                    if (_cellGrid[i, j].GetObject() != null)
                    {
                        objects.Add(_cellGrid[i, j].GetObject());
                    }
                }
            }
        }

        return objects.ToArray();
    }
}