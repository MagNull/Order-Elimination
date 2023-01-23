using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UIManagement;
using UIManagement.Elements;
using UIManagement.trashToRemove_Mockups;
using UnityEngine;
using UnityEngine.UI;

public class BattleDefeatPanel : UIPanel
{
    public override PanelType PanelType => PanelType.BattleDefeat;
    [SerializeField] private PageSwitcher _pageSwitcher;
    [SerializeField] private CharacterAvatarsList _characterList;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TextMeshProUGUI _primaryCurrency;
    public event Action LastContinueButtonPressed;

    public void UpdateBattleResult(BattleResult battleResult)
    {
        _primaryCurrency.text = battleResult.PrimaryCurrencyReceived.ToString();
        _characterList.Clear();
        _characterList.Populate(battleResult.SquadCharacters);

    }

    protected override void Initialize()
    {
        base.Initialize();
        _continueButton.onClick.RemoveListener(OnContineButtonClick);
        _continueButton.onClick.AddListener(OnContineButtonClick);
        _characterList.ElementHolded += OnCharacterAvatarHolded;
    }

    private void OnCharacterAvatarHolded(CharacterClickableAvatar characterAvatar)
    {
        var characterPanel = (CharacterDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
        characterPanel.UpdateCharacterDescription(characterAvatar.CurrentCharacterInfo);
    }

    private void OnContineButtonClick()
    {
        if (!_pageSwitcher.ShowNextAvailablePage(false))
            OnContinueAtLastPagePressed();
    }

    private void OnContinueAtLastPagePressed()
    {
        Debug.Log("«Continue» button pressed on last page");
        LastContinueButtonPressed?.Invoke();
    }
}
