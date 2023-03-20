using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OrderElimination.BM;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using OrderElimination.AbilitySystem;

public class Cell : IActionTarget
{
    public IBattleEntity[] GetContainingEntities() => throw new NotImplementedException();

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
