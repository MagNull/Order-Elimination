using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;
using CharacterAbility;
using Cysharp.Threading.Tasks;
using OrderElimination;
using OrderElimination.BM;
using Sirenix.OdinInspector;
using VContainer;
using UIManagement.Elements;
using UIManagement;
using UnityEngine.Rendering;

public enum BattleState
{
    PlayerTurn,
    EnemyTurn,
    End
}

public class BattleSimulation : SerializedMonoBehaviour
{
    public static event Action PlayerTurnStarted;
    public static event Action EnemyTurnStarted;

    public static event Action<BattleOutcome> BattleEnded;

    private CharacterArrangeDirector _characterArrangeDirector;
    private BattleMapDirector _battleMapDirector;
    private AbilityViewBinder _abilityViewBinder;

    //TODO: Remove panel, use event instead call inside methods(like EndRound, BattleEnd)
    [SerializeField]
    private AbilityPanel _abilityPanel;
    [SerializeField]
    private CharacterBattleStatsPanel _selectedPlayerCharacterStatsPanel;
    [SerializeField]
    private EnemiesListPanel _enemiesListPanel;
    [SerializeField]
    private Dictionary<int, List<Vector2Int>> _unitPositions = new();
    [SerializeField]
    private Dictionary<int, List<Vector2Int>> _enemyPositions = new();

    private BattleOutcome _outcome;

    private static BattleState _battleState;

    private List<BattleCharacter> _characters;

    public static BattleState BattleState => _battleState;

    [Inject]
    private void Construct(CharacterArrangeDirector characterArrangeDirector, BattleMapDirector battleMapDirector)
    {
        _battleMapDirector = battleMapDirector;
        _characterArrangeDirector = characterArrangeDirector;
    }

    private void Start()
    {
        InitializeBattlefield();
        StartRound();
    }

    private void Awake()
    {
        _characters = new List<BattleCharacter>();
        _abilityViewBinder = new AbilityViewBinder();
    }

    private void CheckBattleOutcome()
    {
        if (_outcome != BattleOutcome.Neither)
        {
            return;
        }

        bool isThereAnyAliveAlly = false;
        bool isThereAnyAliveEnemy = false;

        foreach (var unit in _characters)
        {
            if (unit.Stats.Health > 0)
            {
                if (unit.Type == BattleObjectType.Ally)
                {
                    isThereAnyAliveAlly = true;
                }

                if (unit.Type == BattleObjectType.Enemy)
                {
                    isThereAnyAliveEnemy = true;
                }
            }
        }

        _outcome = !isThereAnyAliveAlly
            ? BattleOutcome.Defeat
            : (isThereAnyAliveEnemy ? BattleOutcome.Neither : BattleOutcome.Victory);
        if (_outcome != BattleOutcome.Neither)
            EndBattle();
    }

    public void EndTurn()
    {
        _abilityPanel.ResetAbilityButtons();
        _selectedPlayerCharacterStatsPanel.HideInfo();
        SwitchTurn();
        StartRound();
    }

    private async void StartRound()
    {
        CheckBattleOutcome();
        if (_outcome != BattleOutcome.Neither)
        {
            EndBattle();
            return;
        }

        if (_battleState == BattleState.PlayerTurn)
        {
            Debug.Log("Начался ход игрока" % Colorize.Green);
            PlayerTurnStarted?.Invoke();
        }
        else if (_battleState == BattleState.EnemyTurn)
        {
            EnemyTurnStarted?.Invoke();
            await StartEnemyTurn();
            Debug.Log("End");
            EndTurn();
        }
    }

    private async UniTask StartEnemyTurn()
    {
        Debug.Log("Начался ход ИИ" % Colorize.Red);

        _battleState = BattleState.EnemyTurn;
        var enemies = _characters
            .Select(x => x)
            .Where(x => x.Type == BattleObjectType.Enemy);
        foreach (var enemy in enemies)
        {
            await enemy.PlayTurn();
        }
    }

    private void EndBattle()
    {
        if (_battleState == BattleState.End) 
            return;
        BattleEnded?.Invoke(_outcome);
        _selectedPlayerCharacterStatsPanel.HideInfo();
        _abilityPanel.ResetAbilityButtons();
        _battleState = BattleState.End;
        Debug.LogFormat("�������� ��������� - ������� {0}", _outcome == BattleOutcome.Victory ? "�����" : "��");
    }

    private void SwitchTurn() => _battleState =
        _battleState == BattleState.PlayerTurn ? BattleState.EnemyTurn : BattleState.PlayerTurn;

    private void InitializeBattlefield()
    {
        _battleState = BattleState.PlayerTurn;
        _outcome = BattleOutcome.Neither;

        var mapIndex = _battleMapDirector.InitializeMap();

        _characterArrangeDirector.SetArrangementMap(_battleMapDirector.Map);
        _characters = _characterArrangeDirector.Arrange(_unitPositions[mapIndex], _enemyPositions[mapIndex]);

        var enemies = _characters
            .Where(c => c.Type == BattleObjectType.Enemy)
            .Select(c => c.View.GameObject.GetComponent<BattleCharacterView>())
            .ToArray();
        
        _enemiesListPanel.Populate(enemies);
        foreach (var enemy in enemies)
        {
            enemy.Disabled += _enemiesListPanel.RemoveItem;
            enemy.Disabled += view => _characters.Remove((BattleCharacter) view.Model);
        }

        foreach (var battleCharacter in _characters)
        {
            battleCharacter.Died += _ => CheckBattleOutcome();
        }

        _abilityViewBinder.BindAbilityButtons(_battleMapDirector.MapView, _abilityPanel);

        BattleCharacterView.Selected += _selectedPlayerCharacterStatsPanel.UpdateCharacterInfo;
        BattleCharacterView.Deselected += info => _selectedPlayerCharacterStatsPanel.HideInfo();
    }

    private void OnDisable()
    {
        _abilityViewBinder.OnDisable(_battleMapDirector.MapView, _abilityPanel);
    }
}