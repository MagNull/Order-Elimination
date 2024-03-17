using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameInventory;
using OrderElimination;
using OrderElimination.MacroGame;
using OrderElimination.SavesManagement;
using OrderElimination.UI;
using RoguelikeMap.Map;
using RoguelikeMap.UI.PointPanels;
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

    private SceneTransition _sceneTransition;
    private ScenesMediator _scenesMediator;
    private IPlayerProgressManager _progressManager;
    private Vector3 _startMenuPanelInitialPosition;

    [Inject]
    public void Construct(
        SceneTransition sceneTransition,
        ScenesMediator scenesMediator,
        IPlayerProgressManager progressManager)
    {
        _sceneTransition = sceneTransition;
        _scenesMediator = scenesMediator;
        _progressManager = progressManager;
    }

    private void Start()
    {
        _startMenuPanelInitialPosition = _startMenuPanel.transform.position;
        var progress = _progressManager.GetPlayerProgress();
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

        _startGameButton.onClick.AddListener(StartNewRun);
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
        Logging.Log("Start/Continue run!" % Colorize.Red);
        RoguelikeRunStartManager.StartRun(progress, mediator);
        _sceneTransition.LoadRoguelikeMap();
    }

    private void StartNewRun()
    {
        Debug.Log("Button: Start");
        if (_scenesMediator.Contains<int>(MediatorRegistration.CurrentPoint))
            _scenesMediator.Unregister(MediatorRegistration.CurrentPoint);

        if (PlayerPrefs.HasKey(ShopPanel.BuyedItemsKey))
        {
            PlayerPrefs.DeleteKey(ShopPanel.BuyedItemsKey);
        }

        var progress = _progressManager.GetPlayerProgress();

        var stats = progress.MetaProgress.StatUpgrades;
        var characters = _choosingCharacterPanel.GetSelectedCharacters();
        if (characters.Length == 0)
            return;
        progress.CurrentRunProgress = RoguelikeRunStartManager
            .GetInitialProgress(progress.MetaProgress);
        progress.MetaProgress.StatUpgrades = stats;//TODO-SAVES: make dependency in upgrader
        progress.CurrentRunProgress.PosessedCharacters = characters.ToList();
        OnRunStart(progress, _scenesMediator);
    }
}
