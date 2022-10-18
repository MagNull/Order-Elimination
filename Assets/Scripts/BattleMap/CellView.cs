using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellView : MonoBehaviour
{
    private IBattleObject _object;

    public IBattleObject GetObject()
    {
        if (_object != null)
        {
            return _object;
        }
        return null;
    }

    public void SetObject(IBattleObject obj)
    {
        _object = obj;
    }
}
