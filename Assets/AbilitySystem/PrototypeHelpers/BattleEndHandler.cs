using Cysharp.Threading.Tasks;
using DefaultNamespace;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Battle;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Linq;
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
        TextEmitter textEmitter)
    {
        _battleContext = battleContext;
        _textEmitter = textEmitter;
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
        //_textEmitter.Emit($"Нажмите «Esc» для выхода.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
    }

    private async void OnPlayerVictory()
    {
        await OnBattleEnded();
        //_textEmitter.Emit($"Победа людей.", Color.green, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
        var playerCharacters = SquadMediator.CharacterList.ToArray();
        var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
        panel.UpdateBattleResult(
            playerCharacters, 
            1337, 
            () => SceneManager.LoadSceneAsync(OnExitSceneId));
        Logging.Log($"Current squad [{playerCharacters.Length}]: {string.Join(", ", playerCharacters.Select(c => c.CharacterData.Name))}" % Colorize.Red);
        SquadMediator.SetCharacters(
            BattleUnloader.UnloadCharacters(_battleContext, playerCharacters));
    }

    private async void OnPlayerLose()
    {
        await OnBattleEnded();
        //_textEmitter.Emit($"Победа монстров.", Color.red, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
        var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
        panel.UpdateBattleResult(
            SquadMediator.CharacterList, 
            1337, 
            () => SceneManager.LoadSceneAsync(OnRetrySceneId),
            () => SceneManager.LoadSceneAsync(OnExitSceneId));
    }
}
