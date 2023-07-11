using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UIManagement.Elements;
using UIManagement;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using GameInventory.Items;
using Sirenix.Utilities;
using OrderElimination;
using UIManagement.Panels;
using UnityEngine.Serialization;

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
    private RectTransform _rewardHolder;
    
    [Title("Prefabs")]
    [AssetsOnly]
    [SerializeField]
    private CharacterClickableAvatar _characterPrefab;
    [SerializeField]
    private RewardItem _rewardItemPrefab;

    private readonly Dictionary<CharacterClickableAvatar, GameCharacter> _charactersByAvatars = new();
    private Action _onAcceptCallback;
    
    public override PanelType PanelType => PanelType.BattleVictory;

    public void UpdateBattleResult(
        IEnumerable<GameCharacter> charactersToDisplay,
        int currencyReward,
        Item[] itemsReward,
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
        var currencyRewardItem = Instantiate(_rewardItemPrefab, _rewardHolder);
        currencyRewardItem.UpdateItemInfo(null, currencyReward.ToString());
        foreach (var item in itemsReward)
        {
            var rewardItem = Instantiate(_rewardItemPrefab, _rewardHolder);
            rewardItem.UpdateItemInfo(item.View.Icon, item.View.Name);
        }

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
