using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBattleCharacterInfo
{
    public BattleStats GetStats();
    public BattleCharacterView GetView();
    public AbilityView[] GetAbilities();
}
