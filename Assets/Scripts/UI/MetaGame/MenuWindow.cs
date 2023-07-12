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
    private Image _maskWallpaper;
    
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
            _maskWallpaper.gameObject.SetActive(true);
            _maskWallpaper.DOFade(0.65f, 1.5f);
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
        _maskWallpaper.DOFade(0, 1.25f)
            .OnComplete(() => _maskWallpaper.gameObject.SetActive(false));
    }

    public void ExitInMenuClick()
    {
        Application.Quit();
    }
}
