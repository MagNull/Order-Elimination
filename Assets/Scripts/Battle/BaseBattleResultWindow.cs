using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using StartSessionMenu;
using StartSessionMenu.ChooseCharacter;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BaseBattleResultWindow : MonoBehaviour
{
    [SerializeField]
    private Button _nextButton;
    [SerializeField] 
    private ChoosingCharacter _choosingCharacterPanel;
    [SerializeField]
    private MetaShop _metaShopPanel;
    
    private SceneTransition _sceneTransition;
    
    [Inject]
    public void Construct(SceneTransition sceneTransition)
    {
        _sceneTransition = sceneTransition;
    }
    
    private void Start()
    {
        _nextButton.onClick.AddListener(() =>
        {
            _metaShopPanel.SaveStats();
            if(_choosingCharacterPanel.SaveCharacters())
                _sceneTransition.LoadRoguelikeMap();
        });
    }
}
