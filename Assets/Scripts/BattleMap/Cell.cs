using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.BattleMap;

public class Cell
{
    private IBattleObject _object;

    public Cell()
    {
        _object = new NullBattleObject();
    }

    public IBattleObject GetObject()
    {
        return _object;
    }

    public void SetObject(IBattleObject obj)
    {
        _object = obj;
    }
}
