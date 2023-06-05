using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UnityEngine;
using VContainer;

using UIManagement.trashToRemove_Mockups;
using OrderElimination;
using System.Linq;

public class BattleEndHandler : MonoBehaviour
{
    private SceneTransition _sceneTransition;
    private CharactersMediator _mediator;

    [Inject]
    public void Construct(SceneTransition sceneTransition, CharactersMediator mediator)
    {
        _sceneTransition = sceneTransition;
        _mediator = mediator;
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
        Character[] allies = null;//_mediator.GetPlayerCharactersInfo().Cast<Character>().ToArray();
        var currentPlanetInfo = _mediator.PlanetInfo;
        var battleResultInfo = new BattleResult(outcome, allies, currentPlanetInfo.CurrencyReward, 0);
        if (outcome == BattleOutcome.Victory)
        {
            var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
            panel.UpdateBattleResult(battleResultInfo);
            panel.LastContinueButtonPressed -= _sceneTransition.LoadStrategyMap;
            panel.LastContinueButtonPressed += _sceneTransition.LoadStrategyMap;
        }
        else
        {
            var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
            panel.UpdateBattleResult(battleResultInfo);
            panel.LastContinueButtonPressed -= _sceneTransition.LoadBattleMap;
            panel.LastContinueButtonPressed += _sceneTransition.LoadBattleMap;
        }
    }
}