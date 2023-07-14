using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderElimination;
using OrderElimination.MacroGame;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using UIManagement;
using UIManagement.Elements;
using UIManagement.Panels;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BattleDefeatPanel : UIPanel
{
    [Title("Components")]
    [SerializeField] 
    private PageSwitcher _pageSwitcher;
    [SerializeField] 
    private Button _retryButton;
    [SerializeField]
    private Button _surrenderButton;
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
    private Action _onRetryCallback;
    private Action _onSurrenderCallback;

    public override PanelType PanelType => PanelType.BattleDefeat;

    public void UpdateBattleResult(
        IEnumerable<GameCharacter> charactersToDisplay,
        int currencyReward,
        Action onRetryCallback,
        Action onSurrenderCallback)
    {
        ClearCharacters();
        _onRetryCallback = onRetryCallback;
        _onSurrenderCallback = onSurrenderCallback;
        if (charactersToDisplay != null)
        {
            foreach (var character in charactersToDisplay)
            {
                var avatar = Instantiate(_characterPrefab, _charactersHolder);
                avatar.UpdateCharacterInfo(character.CharacterData.Name, character.CharacterData.BattleIcon);
                avatar.IsClickable = true;
                avatar.Clicked += OnAvatarClicked;
                _charactersByAvatars.Add(avatar, character);
            }
        }
        var currencyRewardItem = Instantiate(_rewardItemPrefab, _rewardHolder);
        currencyRewardItem.UpdateItemInfo(null, currencyReward.ToString());
        _pageSwitcher.DisablePage(1);

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
        _retryButton.onClick.RemoveListener(OnRetry);
        _retryButton.onClick.AddListener(OnRetry);
        _surrenderButton.onClick.RemoveListener(OnSurrender);
        _surrenderButton.onClick.AddListener(OnSurrender);
    }

    private void OnAvatarClicked(CharacterClickableAvatar avatar)
    {
        var characterPanel = (CharacterDescriptionPanel)UIController.SceneInstance.OpenPanel(PanelType.CharacterDescription);
        characterPanel.UpdateCharacterDescription(_charactersByAvatars[avatar]);
    }

    private void OnRetry()
    {
        _onRetryCallback?.Invoke();
    }

    private void OnSurrender()
    {
        _onSurrenderCallback?.Invoke();
    }
}
