using System.Collections;
using System.Collections.Generic;
using OrderElimination;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class BaseBattleResultWindow : MonoBehaviour
{
    [SerializeField]
    private Button _nextButton;
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
            _sceneTransition.LoadRoguelikeMap();
        });
    }
    
}
