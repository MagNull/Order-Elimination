using Cysharp.Threading.Tasks;
using DG.Tweening;
using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using VContainer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BattleEntityView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    private BattleMapView _battleMapView;
    private Vector3 _currentUnanimatedPosition;

    public IAbilitySystemActor BattleEntity { get; private set; }
    public string Name { get; private set; }
    public Sprite BattleIcon { get; private set; }

    [Inject]
    public void Construct(BattleMapView battleMapView)
    {
        _battleMapView = battleMapView;
    }

    public void Initialize(IAbilitySystemActor entity, Sprite battleIcon, string name)
    {
        if (BattleEntity != null)
            return;
        BattleEntity = entity;
        _renderer.sprite = BattleIcon = battleIcon;
        Name = $"{entity.BattleSide} {entity.EntityType} «{name}»";
        BattleEntity.MovedFromTo += OnMoved;
    }

    public void EntityPlacedOnMapCallback()
    {
        if (!BattleEntity.DeployedBattleMap.Contains(BattleEntity))
            throw new System.InvalidOperationException("Enemy hasn't been located on the map yet.");
        var position = BattleEntity.Position;
        _currentUnanimatedPosition = _battleMapView.GetCell(position.x, position.y).transform.position;
        transform.position = _currentUnanimatedPosition;
    }

    public async UniTask Shake(float strength)
    {
        transform.DOComplete();
        strength = Mathf.Clamp01(strength);
        var duration = 1f;
        var shakeX = 0.5f;
        var shakeY = 0.5f;
        transform.DOShakePosition(duration, new Vector3(shakeX, shakeY, 0) * strength);
    }

    private void OnMoved(Vector2Int from, Vector2Int to)
    {
        //var realWorldStartPos = _battleMapView.GetCell(from.x, from.y).transform.position;
        _currentUnanimatedPosition = _battleMapView.GetCell(to.x, to.y).transform.position;
        //transform.position = realWorldStartPos;
        //transform.DOMove(realWorldEndPos, MoveTime);
        transform.position = _currentUnanimatedPosition;
    }

    private void OnEnable()
    {
        if (BattleEntity != null)
            BattleEntity.MovedFromTo += OnMoved;
    }

    private void OnDisable()
    {
        if (BattleEntity != null)
            BattleEntity.MovedFromTo -= OnMoved;
    }
}
