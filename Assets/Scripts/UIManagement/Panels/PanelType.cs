using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIManagement
{
    public enum PanelType
    {
        _BasePanel,
        Pause, 
        Order,
        SquadList,
        ExplorationResult, // Exploration - исследование // Scouting - разведка
        BattleVictory, 
        BattleDefeat, 
        CharacterDescription, 
        CharacterUpgradable, 
        AbilityDescription,
        PassiveSkillsDescription,
        EffectsDesriptionList
    }
}
