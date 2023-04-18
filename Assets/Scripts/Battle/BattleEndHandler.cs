using System;
using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UnityEngine;
using VContainer;

using UIManagement.trashToRemove_Mockups;
using OrderElimination;
using System.Linq;
using RoguelikeMap;

public class BattleEndHandler : MonoBehaviour
{
    private int CurrencyReward = 300;
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
        var allies = _mediator.GetBattleCharactersInfo().Cast<Character>().ToArray();
        var battleResultInfo = new BattleResult(outcome, allies, CurrencyReward, 0);
        if (outcome == BattleOutcome.Victory)
        {
            var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
            panel.UpdateBattleResult(battleResultInfo);
            panel.LastContinueButtonPressed -= _sceneTransition.LoadRoguelikeMap;
            panel.LastContinueButtonPressed += _sceneTransition.LoadRoguelikeMap;
        }
        else
        {
            var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
            panel.UpdateBattleResult(battleResultInfo);
            //TODO(coder): open ChoosingCharacterScreen when lose battle
            // var action = new Action(() =>((ChoosingCharacter)UIController.SceneInstance
            //         .OpenPanel(PanelType.SquadMembers))
            //     .UpdateCharacterInfo(allies.ToList(), true));
            //panel.LastContinueButtonPressed -= action;
            //panel.LastContinueButtonPressed += action;
        }
    }

    public void ReloadGame()
    {
        _sceneTransition.LoadBattleMap();
    }
}