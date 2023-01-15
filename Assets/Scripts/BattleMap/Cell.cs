using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.BattleMap;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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
