using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleButton : MonoBehaviour
{
    [SerializeField] private Button _button;

    public void OnEnable()
    {
        BattleSimulation.BattleEnded += TurnButtonOff;
    }

    public void OnDisable()
    {
        BattleSimulation.BattleEnded -= TurnButtonOff;
    }

    public void TurnButtonOff(BattleOutcome outcome)
    {
        _button.enabled = false;
    }
}
