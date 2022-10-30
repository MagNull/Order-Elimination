using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleObject
{
    public BattleCharacterView GetView();
    public void OnTurnStart();
}
