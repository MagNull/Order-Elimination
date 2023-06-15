using Cysharp.Threading.Tasks;
using DefaultNamespace;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using UIManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

public class BattleEndHandler : MonoBehaviour
{
    [HideInInspector, SerializeField]
    private int _onPlayerVictorySceneId;
    [HideInInspector, SerializeField]
    private int _onPlayerLoseSceneId;
    private IReadOnlyEntitiesBank _entitiesBank;
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

    public int OnExitScene
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
    public int OnRetryScene
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
        IReadOnlyEntitiesBank entitiesBank, 
        TextEmitter textEmitter)
    {
        _entitiesBank = entitiesBank;
        _textEmitter = textEmitter;
        battleContext.BattleStarted -= StartTrackingBattle;
        battleContext.BattleStarted += StartTrackingBattle;
    }

    private void StartTrackingBattle(IBattleContext battleContext)
    {
        _entitiesBank.BankChanged -= OnEntitiesBankChanged;
        _entitiesBank.BankChanged += OnEntitiesBankChanged;
    }

    private void OnEntitiesBankChanged(IReadOnlyEntitiesBank bank)
    {
        if (bank.GetEntities(BattleSide.Enemies).Length == 0)
        {
            OnPlayerVictory();
        }
        else if (bank.GetEntities(BattleSide.Player).Length == 0)
        {
            OnPlayerLose();
        }
    }

    private void OnPlayerVictory()
    {
        _textEmitter.Emit($"Победа людей.", Color.green, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
        _textEmitter.Emit($"Нажмите «Esc» для выхода.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
        var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
        panel.UpdateBattleResult(
            SquadMediator.CharacterList, 
            1337, 
            () => SceneManager.LoadSceneAsync(OnExitScene));
    }

    private void OnPlayerLose()
    {
        _textEmitter.Emit($"Победа монстров.", Color.red, new Vector3(0, 1, -1), Vector3.zero, 1.2f, 100, fontSize: 2f);
        _textEmitter.Emit($"Нажмите «Esc» для выхода.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
        var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
        panel.UpdateBattleResult(
            SquadMediator.CharacterList, 
            1337, 
            () => SceneManager.LoadSceneAsync(OnRetryScene),
            () => SceneManager.LoadSceneAsync(OnExitScene));
    }
}
