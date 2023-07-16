using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DefaultNamespace;
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
    private TextEmitter _textEmitter;
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
        TextEmitter textEmitter,
        ScenesMediator scenesMediator,
        ItemsPool itemsPool)
    {
        _itemsPool = itemsPool;
        _battleContext = battleContext;
        _textEmitter = textEmitter;
        _scenesMediator = scenesMediator;
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
        if (bank
            .GetActiveEntities(BattleSide.Enemies)
            .Where(e => e.EntityType == EntityType.Character)
            .Count() == 0)
        {
            OnPlayerVictory();
        }
        else if (bank
            .GetActiveEntities(BattleSide.Player)
            .Where(e => e.EntityType == EntityType.Character)
            .Count() == 0)
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
        var battleResult = CalculateBattleResult(BattleOutcome.Win);
        var playerCharacters = _scenesMediator.Get<IEnumerable<GameCharacter>>("player characters").ToArray();
        var panel = (BattleVictoryPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleVictory);
        panel.UpdateBattleResult(
            playerCharacters, 
            battleResult.MoneyReward, 
            battleResult.ItemsReward,
            () => TryLoadScene(OnExitSceneId));
        Logging.Log($"Current squad [{playerCharacters.Length}]: {string.Join(", ", playerCharacters.Select(c => c.CharacterData.Name))}" % Colorize.Red);
        _scenesMediator.Register("player characters", BattleUnloader.UnloadCharacters(_battleContext, playerCharacters));
        _scenesMediator.Register("battle results", battleResult);
        //GameCharacterSerializer.SaveCharacter(playerCharacters.First());
    }

    private async void OnPlayerLose()
    {
        await OnBattleEnded();
        var battleResult = CalculateBattleResult(BattleOutcome.Lose);
        var playerCharacters = _scenesMediator.Get<IEnumerable<GameCharacter>>("player characters").ToArray();
        var panel = (BattleDefeatPanel)UIController.SceneInstance.OpenPanel(PanelType.BattleDefeat);
        panel.UpdateBattleResult(
            _scenesMediator.Get<IEnumerable<GameCharacter>>("player characters"), 
            battleResult.MoneyReward, 
            () => TryLoadScene(OnRetrySceneId),
            () => TryLoadScene(OnExitSceneId));
        _scenesMediator.Register("player characters", BattleUnloader.UnloadCharacters(_battleContext, playerCharacters));
        _scenesMediator.Register("battle results", battleResult);
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

        var itemsCount = _scenesMediator.Get<int>("items count");
        var items = new Item[itemsCount];
        for (var i = 0; i < itemsCount; i++)
        {
            items[i] = _itemsPool.GetRandomItem();
        }

        battleResult.ItemsReward = items;
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
