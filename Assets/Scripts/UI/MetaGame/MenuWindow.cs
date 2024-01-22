using System.Collections.Generic;
using DG.Tweening;
using GameInventory;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
using OrderElimination.UI;
using RoguelikeMap.Map;
using StartSessionMenu;
using StartSessionMenu.ChooseCharacter;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class MenuWindow : MonoBehaviour
{
    [SerializeField] 
    private Button _previousButton;
    [SerializeField]
    private Button _startGameButton;
    [SerializeField]
    private Button _continueButton;
    [SerializeField] 
    private ChoosingCharacter _choosingCharacterPanel;
    [SerializeField]
    private MetaShop _metaShopPanel;
    [SerializeField] 
    private GameObject _startMenuPanel;
    [SerializeField] 
    private Image _maskWallpaper;

    [SerializeField]
    private IPlayerProgress _defaultPlayerProgress;
    
    private SceneTransition _sceneTransition;
    private ScenesMediator _scenesMediator;
    private Vector3 _startMenuPanelInitialPosition;
    
    [Inject]
    public void Construct(SceneTransition sceneTransition, ScenesMediator scenesMediator)
    {
        _sceneTransition = sceneTransition;
        _scenesMediator = scenesMediator;
    }
    
    private void Start()
    {
        _startMenuPanelInitialPosition = _startMenuPanel.transform.position;
        var progress = PlayerProgressManager.LoadSavedProgress();
        //Progress validation
        _continueButton.DOInterectable(progress.CurrentRunProgress != null);
        _scenesMediator.Register("progress", progress);

        _previousButton.onClick.AddListener(() =>
        {
            Debug.Log("Button: Prev");
            _startMenuPanel.transform.DOMoveX(_startMenuPanelInitialPosition.x, 1.5f);
            _maskWallpaper.gameObject.SetActive(true);
            _maskWallpaper.DOFade(0.65f, 1.5f);
        });
        
        _continueButton.onClick.AddListener(() =>
        {
            Debug.Log("Button: Continue");
            if (progress.CurrentRunProgress == null)
            {
                throw new System.InvalidOperationException("Should not be allowed");
            }
            _sceneTransition.LoadRoguelikeMap();
            //TODO-SAVE: load progress
        });
        
        _startGameButton.onClick.AddListener(() =>
        {
            Debug.Log("Button: Start");
            if (_scenesMediator.Contains<int>("point index"))
                _scenesMediator.Unregister("point index");

            var stats = _metaShopPanel.GetUpgradeStats();
            var characters = _choosingCharacterPanel.GetSelectedCharacters();
            if (characters.Length == 0)
                return;
            _scenesMediator.Register("stats", stats);
            _scenesMediator.Register("player characters", characters);
            progress.CurrentRunProgress = new()
            {
                PosessedCharacters = characters,
                StatUpgrades = stats,
                RoguelikeCurrency = 1800//TODO: Get from MetaProgress
            };
            _sceneTransition.LoadRoguelikeMap();
        });
    }

    public void StartInMenuClick()
    {
        _startMenuPanel.transform.DOMoveX(-1920, 1.5f);
        _maskWallpaper.DOFade(0, 1.25f)
            .OnComplete(() => _maskWallpaper.gameObject.SetActive(false));
    }

    public void ExitInMenuClick()
    {
        Application.Quit();
    }
}
