using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UnityEngine;
using VContainer;

using UIManagement.trashToRemove_Mockups;

public class BattleEndHandler : MonoBehaviour
{
    [SerializeField]
    private BaseBattleResultWindow _winWindow;
    [SerializeField]
    private BaseBattleResultWindow _defeatWindow;
    private IObjectResolver _objectResolver;

    [Inject]
    public void Construct(IObjectResolver objectResolver)
    {
        _objectResolver = objectResolver;
        _winWindow.gameObject.SetActive(false);
        _defeatWindow.gameObject.SetActive(false);
    }

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
        Debug.Log(outcome);

        var battleResultInfo = new BattleResult(outcome, null, 0, 0);
        if (outcome == BattleOutcome.Victory)
        {
            var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
            panel.UpdateBattleResult(battleResultInfo);
        }
        else
        {
            var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
            panel.UpdateBattleResult(battleResultInfo);
        }

        var results = outcome == BattleOutcome.Victory ? _winWindow : _defeatWindow;
        results.gameObject.SetActive(true);
        _objectResolver.Inject(results);
    }
}