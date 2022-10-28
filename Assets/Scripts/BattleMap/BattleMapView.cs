using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMapView : MonoBehaviour
{
    [SerializeField]
    private BattleMap _battleMap;

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
                if (x + i >= 0 && x + i < 8 && y + j >=0 && y + j < 8)
                {
                    // Клетка[x+i, y+i].Light();
                }
            }
        }
    }

    public void OnCellChanged(CellView cell)
    {
        // IBattleObject -> BattleObject не круто
        BattleObject obj = cell.GetObject() as BattleObject;
        obj.gameObject.transform.position = cell.transform.position;
        Debug.Log("Произошло событие!");
    }
}
