using DG.Tweening;
using OrderElimination.AbilitySystem;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using VContainer;

public class BattleEntityView : MonoBehaviour
{
    public float MoveTime;
    [SerializeField]
    private SpriteRenderer _renderer;
    private BattleMapView _battleMapView;

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
        transform.position = _battleMapView.GetCell(position.x, position.y).transform.position;
    }

    private void OnMoved(Vector2Int from, Vector2Int to)
    {
        var realWorldPos = _battleMapView.GetCell(to.x, to.y).transform.position;
        transform.DOMove(realWorldPos, MoveTime);
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
