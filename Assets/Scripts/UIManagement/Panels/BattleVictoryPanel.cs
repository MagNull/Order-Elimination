using OrderElimination.MetaGame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UIManagement.Elements;
using UIManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using Sirenix.Utilities;
using OrderElimination;

public class BattleVictoryPanel : UIPanel
{
    [Title("Components")]
    [SerializeField]
    private PageSwitcher _pageSwitcher;
    [SerializeField]
    private Button _continueButton;
    [SerializeField]
    private RectTransform _charactersHolder;
    [SerializeField]
    private TextMeshProUGUI _primaryCurrency;
    [Title("Prefabs")]
    [AssetsOnly]
    [SerializeField]
    private CharacterClickableAvatar _characterPrefab;

    private readonly Dictionary<CharacterClickableAvatar, GameCharacter> _charactersByAvatars = new();
    private Action _onAcceptCallback;
    
    public override PanelType PanelType => PanelType.BattleVictory;

    public void UpdateBattleResult(
        IEnumerable<GameCharacter> charactersToDisplay,
        int currencyReward,
        Action onAcceptCallback)
    {
        ClearCharacters();
        _onAcceptCallback = onAcceptCallback;
        foreach (var character in charactersToDisplay)
        {
            var avatar = Instantiate(_characterPrefab, _charactersHolder);
            avatar.UpdateCharacterInfo(character.CharacterData.Name, character.CharacterData.BattleIcon);
            avatar.IsClickable = true;
            avatar.Clicked += OnAvatarClicked;
            _charactersByAvatars.Add(avatar, character);
        }
        _primaryCurrency.text = currencyReward.ToString();

        void ClearCharacters()
        {
            foreach (var avatar in _charactersByAvatars.Keys)
            {
                avatar.Clicked -= OnAvatarClicked;
            }
            var elementsToDestroy = _charactersByAvatars.Keys.ToArray();
            _charactersByAvatars.Clear();
            elementsToDestroy.ForEach(a => Destroy(a));
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        _continueButton.onClick.RemoveListener(OnContineButtonClick);
        _continueButton.onClick.AddListener(OnContineButtonClick);
    }

    private void OnAvatarClicked(CharacterClickableAvatar avatar)
    {
        Logging.Log("Clicked character");
        var characterPanel = (CharacterDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
        characterPanel.UpdateCharacterDescription(_charactersByAvatars[avatar]);
    }

    private void OnContineButtonClick()
    {
        if (!_pageSwitcher.ShowNextAvailablePage(false))
            OnContinueAtLastPagePressed();
    }

    private void OnContinueAtLastPagePressed()
    {
        Logging.Log("«Continue» button pressed on last page", context: this);
        _onAcceptCallback?.Invoke();
    }
}
