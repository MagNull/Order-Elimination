using System.Collections.Generic;
using System.Linq;
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
            OnRunStart(progress, _scenesMediator);
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
            progress.MetaProgress = new()
            {
                StatUpgrades = stats,
                MetaCurrency = 1777,
                HireCurrencyLimit = 1111
            };
            progress.CurrentRunProgress = new()
            {
                PosessedCharacters = characters.ToList(),
                RoguelikeCurrency = 1800//TODO: Get from MetaProgress
            };
            OnRunStart(progress, _scenesMediator);
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

    private void OnRunStart(IPlayerProgress progress, ScenesMediator mediator)
    {
        RoguelikeRunStartManager.StartNewRun(progress, mediator);
        _sceneTransition.LoadRoguelikeMap();
    }
}
