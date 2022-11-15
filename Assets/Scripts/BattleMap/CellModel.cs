using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.BattleMap;

public class CellModel
{
    private IBattleObject _object;

    public CellModel()
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
