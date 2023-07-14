using System.Collections.Generic;
using DG.Tweening;
using OrderElimination;
using OrderElimination.MacroGame;
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
    
    private SceneTransition _sceneTransition;
    private ScenesMediator _scenesMediator;
    
    [Inject]
    public void Construct(SceneTransition sceneTransition, ScenesMediator scenesMediator)
    {
        _sceneTransition = sceneTransition;
        _scenesMediator = scenesMediator;
    }
    
    private void Start()
    {
        _continueButton
            .DOInterectable(_scenesMediator.Contains<IEnumerable<GameCharacter>>("player characters"));

        _previousButton.onClick.AddListener(() =>
        {
            _startMenuPanel.transform.DOMoveX(960, 1.5f);
            _maskWallpaper.gameObject.SetActive(true);
            _maskWallpaper.DOFade(0.65f, 1.5f);
        });
        
        _continueButton.onClick.AddListener(() =>
        {
            _sceneTransition.LoadRoguelikeMap();
        });
        
        _startGameButton.onClick.AddListener(() =>
        {
            _metaShopPanel.SaveStats();
            PlayerPrefs.DeleteKey(Map.SquadPositionKey);
            if(_choosingCharacterPanel.SaveCharacters())
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
