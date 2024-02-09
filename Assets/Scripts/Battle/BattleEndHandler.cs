using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.Battle;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System.Linq;
using GameInventory.Items;
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
    private ScenesMediator _scenesMediator;
    private ItemsPool _itemsPool;
    private bool _isLoadingNextScene;

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
        ScenesMediator scenesMediator,
        ItemsPool itemsPool)
    {
        _itemsPool = itemsPool;
        _battleContext = battleContext;
        _scenesMediator = scenesMediator;
        battleContext.BattleStarted -= StartTrackingBattle;
        battleContext.BattleStarted += StartTrackingBattle;
    }

    private void StartTrackingBattle(IBattleContext battleContext)
    {
        battleContext.BattleRules.VictoryTracker.ConditionMet += OnVictoryConditionMet;
        battleContext.BattleRules.DefeatTracker.ConditionMet += OnDefeatConditionMet;
        battleContext.BattleRules.VictoryTracker.StartTracking(battleContext);
        battleContext.BattleRules.DefeatTracker.StartTracking(battleContext);
    }

    private void OnVictoryConditionMet(IBattleTracker tracker)
    {
        OnPlayerVictory();
    }

    private void OnDefeatConditionMet(IBattleTracker tracker)
    {
        OnPlayerLose();
    }

    private void OnBattleEnded(BattleOutcome battleOutcome)
    {
        _battleContext.BattleRules.VictoryTracker.StopTracking();
        _battleContext.BattleRules.DefeatTracker.StopTracking();
        _battleContext.BattleRules.VictoryTracker.ConditionMet -= OnVictoryConditionMet;
        _battleContext.BattleRules.DefeatTracker.ConditionMet -= OnDefeatConditionMet;
        if (_playerControls != null)
            _playerControls.enabled = false;
        var playerCharacters = _scenesMediator.Get<GameCharacter[]>(MediatorRegistration.PlayerCharacters);
        Logging.Log($"Current squad [{playerCharacters.Length}]: {string.Join(", ", playerCharacters.Select(c => c.CharacterData.Name))}" % Colorize.Red);
        var battleResult = CalculateBattleResult(battleOutcome);

        _scenesMediator.Register(
            MediatorRegistration.PlayerCharacters, 
            BattleUnloader.UnloadCharacters(_battleContext, playerCharacters));
        _scenesMediator.Register(MediatorRegistration.BattleResults, battleResult);
        //_textEmitter.Emit($"������� �Esc� ��� ������.", Color.white, new Vector3(0, -1, -1), Vector3.zero, 1.2f, 100, fontSize: 0.75f);
    }

    private async void OnPlayerVictory()
    {
        OnBattleEnded(BattleOutcome.Win);
        var playerCharacters = _scenesMediator.Get<GameCharacter[]>(MediatorRegistration.PlayerCharacters).ToArray();
        var battleResult = _scenesMediator.Get<BattleResults>(MediatorRegistration.BattleResults);
        await UniTask.Delay(Mathf.RoundToInt(BattleResultsDisplayDelay * 1000));
        var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
        panel.UpdateBattleResult(
            playerCharacters, 
            battleResult.MoneyReward, 
            battleResult.ItemsReward,
            () => TryLoadScene(OnExitSceneId));
        //GameCharacterSerializer.SaveCharacter(playerCharacters.First());
    }

    private async void OnPlayerLose()
    {
        OnBattleEnded(BattleOutcome.Lose); 
        var playerCharacters = _scenesMediator.Get<GameCharacter[]>(MediatorRegistration.PlayerCharacters).ToArray();
        var battleResult = _scenesMediator.Get<BattleResults>(MediatorRegistration.BattleResults);
        await UniTask.Delay(Mathf.RoundToInt(BattleResultsDisplayDelay * 1000));
        var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
        panel.UpdateBattleResult(
            playerCharacters,
            battleResult.MoneyReward, 
            () => TryLoadScene(OnRetrySceneId),
            () => TryLoadScene(OnExitSceneId));
    }

    private BattleResults CalculateBattleResult(BattleOutcome battleOutcome)
    {
        var defeatEnemies = _battleContext.EntitiesBank.GetDisposedEntities()
            .Where(en => en.BattleSide == BattleSide.Enemies);
        var moneyReward = defeatEnemies
            .Aggregate(0, (i, actor) => i + _battleContext.EntitiesBank.GetBasedCharacter(actor).CharacterData.Reward);
        
        var battleResult = new BattleResults
        {
            BattleOutcome = battleOutcome,
            MoneyReward = moneyReward,
        };
        if (battleOutcome == BattleOutcome.Lose)
            return battleResult;

        var allItems = _scenesMediator.Get<Dictionary<Item, float>>(MediatorRegistration.RewardItems);
        var resultItems = new List<Item>();
        foreach (var itemProb in allItems)
        {
            var roll = Random.Range(1, 100);
            if(roll <= itemProb.Value)
                resultItems.Add(itemProb.Key);
        }

        battleResult.ItemsReward = resultItems.ToArray();
        return battleResult;
    }

    private async void TryLoadScene(int sceneId)
    {
        if (_isLoadingNextScene)
            return;
        _isLoadingNextScene = true;
        await SceneManager.LoadSceneAsync(sceneId);
        _isLoadingNextScene = false;
    }
}
