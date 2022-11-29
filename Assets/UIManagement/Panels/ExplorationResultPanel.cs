using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

public class ExplorationResultPanel : UIPanel
{
    public override PanelType PanelType => PanelType.ExplorationResult;
    [SerializeField] private IconTextValueElement _primaryCurrency;
    [SerializeField] private IconTextValueElement _specialCurrency;
    [SerializeField] private Button _continueButton;
    [Header("Повышение опыта")]
    [SerializeField] private PageSwitcher _pageSwitcher;
    [SerializeField] private CharacterList _characterExperienceList;
    [Header("Получение усилений")]
    [SerializeField] private Transform _powerupsScrollList;
    [SerializeField] private PowerupElement _powerupPrefab;

    public void UpdateExplorationResult(ExplorationResult explorationResult)
    {
        _primaryCurrency.Value = $"+{explorationResult.PrimaryCurrencyRecieved}";
        _specialCurrency.Value = $"+{explorationResult.SpecialCurrencyRecieved}";
        _characterExperienceList.HasExperienceRecieved = true;
        _characterExperienceList.HasMaintenanceCost = false;
        _characterExperienceList.HasParameters = false;
        _characterExperienceList.Add(explorationResult.SquadCharacters.ToArray());
        foreach (var c in _characterExperienceList)
        {
            c.ExperienceRecieved = $"+{explorationResult.ExperienceAmount}";
            c.IsDead = c.IsDead;
        }
        foreach (var p in explorationResult.PowerupsRecieved)
        {
            var powerup = Instantiate(_powerupPrefab, _powerupsScrollList);
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        _continueButton.onClick.RemoveAllListeners();
        _continueButton.onClick.AddListener(OnContineButtonClick);
    }

    private void OnContineButtonClick()
    {
        if (!_pageSwitcher.ShowNextAvailablePage(false))
            OnContinueAfterLastPagePressed();
    }

    private void OnContinueAfterLastPagePressed()
    {
        Debug.Log("«Continue» button pressed on last page");
    }

    #region ToRemove
    public void Awake()
    {
        UpdateExplorationResult(new ExplorationResult());
        foreach (var p in _powerupsScrollList.GetComponentsInChildren<PowerupElement>())
        {
            p.ImageComponent.color = Random.ColorHSV(0, 1, 0.8f, 1f, 0.8f, 1f);
        }
    }

    #endregion ToRemove
}
