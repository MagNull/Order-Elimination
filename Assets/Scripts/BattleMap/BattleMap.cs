using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Пора разгрузить метод
public class BattleMap : MonoBehaviour
{
    public static event Action<CellView> CellChanged;

    [SerializeField] private CellGridGenerator _generator;

    private CellView[,] _cellGrid;
    private BattleCharacterFactory _characterFactory;

    public void Start()
    {
        // Инициализация
        _characterFactory = GetComponent<BattleCharacterFactory>();

        // Создание игрового поля
        _cellGrid = _generator.GenerateGrid();

        // Создание игровых персонажей
        TestBattleCharacterInfo info = new TestBattleCharacterInfo();

        BattleCharacter player = _characterFactory.Create(info, CharacterSide.Player);
        BattleCharacter enemy_1 = _characterFactory.Create(info, CharacterSide.Enemy);
        BattleCharacter enemy_2 = _characterFactory.Create(info, CharacterSide.Enemy);

        SetCell(2, 3, player);
        SetCell(4, 5, enemy_1);
        SetCell(6, 0, enemy_2);
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
        // Не null, а null-object?
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
        Debug.Log("Объект не найден на поле!");
        return new Vector2Int(-1, -1);
    }
}
