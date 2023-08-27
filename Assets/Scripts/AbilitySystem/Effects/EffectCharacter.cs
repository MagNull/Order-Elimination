using Sirenix.OdinInspector;
using UnityEngine;

namespace OrderElimination.AbilitySystem
{
    [EnumToggleButtons]
    public enum EffectCharacter
    {
        [GUIColor("@Color.blue")]
        Neutral = 0,
        [GUIColor("@Color.green")]
        Positive = 1,
        [GUIColor("@Color.red")]
        Negative = 2
    }
}
