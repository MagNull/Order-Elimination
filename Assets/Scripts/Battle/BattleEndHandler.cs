using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

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
        var results = outcome == BattleOutcome.Victory ? _winWindow : _defeatWindow;
        results.gameObject.SetActive(true);
        _objectResolver.Inject(results);
    }
}