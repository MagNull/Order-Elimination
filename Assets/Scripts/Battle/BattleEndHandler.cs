using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEndHandler : MonoBehaviour
{
    [SerializeField] private BaseBattleResultWindow _winWindow;
    [SerializeField] private BaseBattleResultWindow _defeatWindow;

    public void OnEnable()
    {
        BattleSimulation.BattleEnded += ShowResults;
    }

    public void OnDisable()
    {
        BattleSimulation.BattleEnded -= ShowResults;
    }

    public void ShowResults(BattleOutcome outcome)
    {
        if (outcome == BattleOutcome.Victory)
        {
            _winWindow.View();
        }
        else
        {
            _defeatWindow.View();
        }
    }
}
