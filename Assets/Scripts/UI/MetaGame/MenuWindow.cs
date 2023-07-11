using DG.Tweening;
using OrderElimination;
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
    private ChoosingCharacter _choosingCharacterPanel;
    [SerializeField]
    private MetaShop _metaShopPanel;
    [SerializeField] 
    private GameObject _startMenuPanel;
    [SerializeField] 
    private GameObject _maskWallpaper;
    
    private SceneTransition _sceneTransition;
    
    [Inject]
    public void Construct(SceneTransition sceneTransition)
    {
        _sceneTransition = sceneTransition;
    }
    
    private void Start()
    {
        _previousButton.onClick.AddListener(() =>
        {
            _startMenuPanel.transform.DOMoveX(960, 1.5f);
            _maskWallpaper.SetActive(true);
        });
        
        _startGameButton.onClick.AddListener(() =>
        {
            _metaShopPanel.SaveStats();
            if(_choosingCharacterPanel.SaveCharacters())
                _sceneTransition.LoadRoguelikeMap();
        });
    }

    public void StartInMenuClick()
    {
        _startMenuPanel.transform.DOMoveX(-1920, 1.5f);
        _maskWallpaper.SetActive(false);
    }

    public void ExitInMenuClick()
    {
        Application.Quit();
    }
}
