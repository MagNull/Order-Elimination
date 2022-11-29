using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

public class BattleDefeatPanel : UIPanel
{
    public override PanelType PanelType => PanelType.BattleDefeat;
    [SerializeField] private IconTextValueElement _primaryCurrency;
    [SerializeField] private IconTextValueElement _specialCurrency;
    [SerializeField] private Button _continueButton;
    [Header("Повышение опыта")]
    [SerializeField] private PageSwitcher _pageSwitcher;
    [SerializeField] private CharacterList _characterExperienceList;
    [Header("Получение усилений")]
    [SerializeField] private Transform _powerupsScrollList;
    [SerializeField] private PowerupElement _powerupPrefab;

    public void UpdateExplorationResult(BattleResult battleResult)
    {
        _primaryCurrency.Value = $"+{battleResult.PrimaryCurrencyRecieved}";
        _specialCurrency.Value = $"+{battleResult.SpecialCurrencyRecieved}";
        _characterExperienceList.HasExperienceRecieved = true;
        _characterExperienceList.HasMaintenanceCost = false;
        _characterExperienceList.HasParameters = false;
        _characterExperienceList.Add(battleResult.SquadCharacters.ToArray());
        foreach (var c in _characterExperienceList)
        {
            c.ExperienceRecieved = $"+{battleResult.ExperienceAmount}";
            c.IsDead = c.IsDead;
        }
        foreach (var p in battleResult.PowerupsRecieved)
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
        UpdateExplorationResult(new BattleResult());
        foreach (var p in _powerupsScrollList.GetComponentsInChildren<PowerupElement>())
        {
            p.ImageComponent.color = Random.ColorHSV(0, 1, 0.8f, 1f, 0.8f, 1f);
        }
    }

    #endregion ToRemove
}
