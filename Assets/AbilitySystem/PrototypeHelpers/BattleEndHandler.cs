using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Battle;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Linq;
using OrderElimination.MacroGame;
using UIManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class BattleEndHandler : MonoBehaviour
{
    [SerializeField]
    private BattleMapSelector _playerControls;
    [HideInInspector, SerializeField]
    private int _onPlayerVictorySceneId;
    [HideInInspector, SerializeField]
    private int _onPlayerLoseSceneId;
    private IBattleContext _battleContext;
    private TextEmitter _textEmitter;
    private ScenesMediator _mediator;

    [ShowInInspector]
    private int _safeVictorySceneId
    {
        get => _onPlayerVictorySceneId;
        set
        {
            var maxIdValue = SceneManager.sceneCountInBuildSettings - 1;
            if (value < 0) value = 0;
            if (value > maxIdValue)
                value = maxIdValue;
            _onPlayerVictorySceneId = value;
        }
    }
    [ShowInInspector]
    private int _safeLoseSceneId
    {
        get => _onPlayerLoseSceneId;
        set
        {
            var maxIdValue = SceneManager.sceneCountInBuildSettings - 1;
            if (value < 0) value = 0;
            if (value > maxIdValue)
                value = maxIdValue;
            _onPlayerLoseSceneId = value;
        }
    }
    [field: SerializeField]
    public float BattleResultsDisplayDelay { get; set; }

    public int OnExitSceneId
    {
        get => _onPlayerVictorySceneId;
        set
        {
            var maxIdValue = SceneManager.sceneCountInBuildSettings - 1;
            if (value > maxIdValue || value < 0)
                throw new System.IndexOutOfRangeException($"No scene with id \"{value}\" in build settings!");
            _onPlayerVictorySceneId = value;
        }
    }
    public int OnRetrySceneId
    {
        get => _onPlayerLoseSceneId;
        set
        {
            var maxIdValue = SceneManager.sceneCountInBuildSettings - 1;
            if (value > maxIdValue || value < 0)
                throw new System.IndexOutOfRangeException($"No scene with id \"{value}\" in build settings!");
            _onPlayerLoseSceneId = value;
        }
    }

    [Inject]
    private void Construct(
        IBattleContext battleContext, 
        TextEmitter textEmitter,
        ScenesMediator scenesMediator)
    {
        _battleContext = battleContext;
        _textEmitter = textEmitter;
        _mediator = scenesMediator;
        battleContext.BattleStarted -= StartTrackingBattle;
        battleContext.BattleStarted += StartTrackingBattle;
    }

    private void StartTrackingBattle(IBattleContext battleContext)
    {
        _battleContext.EntitiesBank.BankChanged -= OnEntitiesBankChanged;
        _battleContext.EntitiesBank.BankChanged += OnEntitiesBankChanged;
    }

    private void OnEntitiesBankChanged(IReadOnlyEntitiesBank bank)
    {
        if (bank.GetActiveEntities(BattleSide.Enemies).Length == 0)
        {
            OnPlayerVictory();
        }
        else if (bank.GetActiveEntities(BattleSide.Player).Length == 0)
        {
            OnPlayerLose();
        }
    }

    private async UniTask OnBattleEnded()
    {
        _battleContext.EntitiesBank.BankChanged -= OnEntitiesBankChanged;
        _playerControls.enabled = false;
        await UniTask.Delay(Mathf.RoundToInt(BattleResultsDisplayDelay * 1000));
        //_textEmitter.Emit($"������� �Esc� ��� ������.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
    }

    private async void OnPlayerVictory()
    {
        await OnBattleEnded();
        //_textEmitter.Emit($"������ �����.", Color.green, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
        var playerCharacters = _mediator.Get<GameCharacter[]>("player characters").ToArray();
        var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
        panel.UpdateBattleResult(
            playerCharacters, 
            1337, 
            () => SceneManager.LoadSceneAsync(OnExitSceneId));
        Logging.Log($"Current squad [{playerCharacters.Length}]: {string.Join(", ", playerCharacters.Select(c => c.CharacterData.Name))}" % Colorize.Red);
        _mediator.Register("player characters", BattleUnloader.UnloadCharacters(_battleContext, playerCharacters));
    }

    private async void OnPlayerLose()
    {
        await OnBattleEnded();
        //_textEmitter.Emit($"������ ��������.", Color.red, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
        var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
        panel.UpdateBattleResult(
            _mediator.Get<GameCharacter[]>("player characters"), 
            1337,
            () => SceneManager.LoadSceneAsync(OnRetrySceneId),
            () => SceneManager.LoadSceneAsync(OnExitSceneId));
    }
}
