using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DG.Tweening;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class BattleEntityView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private Transform _floatingNumbersPosition;

    [HideInInspector, SerializeField]
    private float _iconTargetSize = 1.9f;
    private BattleMapView _battleMapView;
    private IParticlesPool _particlesPool;
    private TextEmitter _textEmitter;
    private float _damageCash;
    private float _healthCash;
    private float _armorCash;

    public static float SummingValuesTimeGap { get; private set; } = 0.01f;
    public static readonly Vector3 ErrorPosition = Vector3.zero;

    [ShowInInspector]
    public float IconTargetSize
    {
        get => _iconTargetSize;
        set
        {
            if (value < 0) value = 0;
            _iconTargetSize = value;
            BattleIcon = BattleIcon;
        }
    }
    public AbilitySystemActor BattleEntity { get; private set; }
    public string Name { get; private set; }
    public Sprite BattleIcon
    {
        get => _renderer.sprite;
        set
        {
            _renderer.sprite = value;
            if (value != null)
            {
                var bounds = value.bounds;
                float maxBoundsSide = bounds.size.y >= bounds.size.x ? bounds.size.y : bounds.size.x;
                _renderer.transform.localScale = Vector3.one * IconTargetSize / maxBoundsSide;
            }
        }
    }
    public GameObject VisualModel { get; private set; }

    [Inject]
    public void Construct(BattleMapView battleMapView, IParticlesPool particlesPool, TextEmitter textEmitter)
    {
        _battleMapView = battleMapView;
        _particlesPool = particlesPool;
        _textEmitter = textEmitter;
    }

    public void Initialize(AbilitySystemActor entity, string name, Sprite battleIcon, GameObject model = null)
    {
        if (BattleEntity != null)
            return;

        BattleEntity = entity;

        BattleEntity.DeployedBattleMap.PlacedOnMap -= OnPlaced;
        BattleEntity.DeployedBattleMap.RemovedFromMap -= OnRemoved;
        BattleEntity.MovedFromTo -= OnMoved;
        BattleEntity.Damaged -= OnDamaged;
        BattleEntity.Healed -= OnHealed;
        BattleEntity.Died -= OnDied;
        BattleEntity.DisposedFromBattle -= OnDisposedFromBattle;
        BattleEntity.StatusHolder.StatusAppeared -= OnStatusAppeared;
        BattleEntity.StatusHolder.StatusDisappeared -= OnStatusDisappeared;

        BattleEntity.DeployedBattleMap.PlacedOnMap += OnPlaced;
        BattleEntity.DeployedBattleMap.RemovedFromMap += OnRemoved;
        BattleEntity.MovedFromTo += OnMoved;
        BattleEntity.Damaged += OnDamaged;
        BattleEntity.Healed += OnHealed;
        BattleEntity.Died += OnDied;
        BattleEntity.DisposedFromBattle += OnDisposedFromBattle;
        BattleEntity.StatusHolder.StatusAppeared += OnStatusAppeared;
        BattleEntity.StatusHolder.StatusDisappeared += OnStatusDisappeared;

        BattleIcon = battleIcon;
        gameObject.name = Name = $"{entity.BattleSide} «{name}»";
        if (model != null)
        {
            VisualModel = Instantiate(model, transform);
        }
        if (BattleEntity.DeployedBattleMap.Contains(BattleEntity))
        {
            throw new InvalidOperationException("Initialize EntityView first and place entity on map after.");
            var gamePosition = BattleEntity.Position;
            transform.position = _battleMapView.GetCell(gamePosition.x, gamePosition.y).transform.position;
        }
        else
        {
            transform.position = ErrorPosition;
        }
    }

    public void Highlight(Color color, float highlightTime, float duration, float fadeTime)
    {
        _renderer.DOComplete();
        var initialColor = _renderer.color;
        DOTween.Sequence(_renderer)
            .Append(_renderer.DOColor(color, highlightTime))
            .Append(_renderer.DOColor(initialColor, fadeTime).SetDelay(duration))
            .Play();
        //_renderer.DOBlendableColor(initialColor, fadeTime);
    }

    public async UniTask Shake(float shakeX = 0.5f, float shakeY = 0.5f, float duration = 1, int vibrations = 10)
    {
        _renderer.transform.DOComplete();
        await _renderer.transform.DOShakePosition(duration, new Vector3(shakeX, shakeY, 0), vibrations).AsyncWaitForCompletion();
    }

    private void OnPlaced(AbilitySystemActor entity)
    {
        if (BattleEntity != entity) return;
        PlaceAtGamePosition(entity.Position);
    }

    private void OnRemoved(AbilitySystemActor entity)
    {
        if (BattleEntity != entity) return;
        //transform.position = ErrorPosition;
    }

    private void OnMoved(Vector2Int from, Vector2Int to)
    {
        PlaceAtGamePosition(to);
    }

    private void PlaceAtGamePosition(Vector2Int position)
    {
        transform.position = _battleMapView.GetCell(position.x, position.y).transform.position;
    }

    private async void OnDamaged(DealtDamageInfo damageInfo)
    {
        if (_damageCash > 0)//waiting
        {
            _damageCash += damageInfo.TotalDamage;
            return;
        }
        else//no cash -> start cashing
        {
            _damageCash += damageInfo.TotalDamage;
            await UniTask.Delay(Mathf.RoundToInt(SummingValuesTimeGap * 1000), true);
        }
        var damageValue = _damageCash;
        _damageCash = 0;

        var strength = Mathf.InverseLerp(0, 200, damageValue);
        var shake = Mathf.Lerp(0, 0.5f, strength);
        var position = _floatingNumbersPosition.position;
        _textEmitter.Emit($"{damageValue}", Color.red, position, new Vector2(0.5f, 0.5f));
        Shake(shake, shake, 1, 10);
    }

    private async void OnHealed(HealRecoveryInfo healInfo)
    {
        if (_healthCash > 0 || _armorCash > 0)//waiting
        {
            _healthCash += healInfo.RecoveredHealth;
            _armorCash += healInfo.RecoveredArmor;
            return;
        }
        else//no cash -> start cashing
        {
            _healthCash += healInfo.RecoveredHealth;
            _armorCash += healInfo.RecoveredArmor;
            await UniTask.Delay(Mathf.RoundToInt(SummingValuesTimeGap * 1000), true);
        }
        var healthValue = _healthCash;
        var armorValue = _armorCash;
        _healthCash = 0;
        _armorCash = 0;

        var position = _floatingNumbersPosition.position;
        var offset = Vector3.up * 0.25f;
        _textEmitter.Emit($"+{armorValue}", Color.cyan, position + offset, new Vector2(0.5f, 0.5f), duration: 1);
        _textEmitter.Emit($"+{healthValue}", Color.green, position - offset, new Vector2(0.5f, 0.5f), duration: 1);
        //Shake(0, 0.07f, 1.5f, 3);
    }

    private void OnDied(AbilitySystemActor entity)
    {
        //var luminosity = 0.1f;
        ////_renderer.DOFade(0.7f, 1).SetEase(Ease.InBounce);
        //_renderer.DOColor(new Color(luminosity, luminosity, luminosity), 0.4f);
        //_renderer.DOFade(0, 0.3f).SetDelay(0.4f).SetEase(Ease.OutBounce);
    }

    private async void OnDisposedFromBattle(IBattleDisposable entity)
    {
        var luminosity = 0.1f;
        //_renderer.DOFade(0.7f, 1).SetEase(Ease.InBounce);
        var tasks = new List<Task>
        {
            _renderer.DOColor(new Color(luminosity, luminosity, luminosity), 0.4f).AsyncWaitForCompletion(),
            _renderer.DOFade(0, 0.3f).SetDelay(0.4f).SetEase(Ease.OutBounce).AsyncWaitForCompletion()
        };
        await Task.WhenAll(tasks);
        gameObject.SetActive(false);
    }

    private void OnStatusAppeared(BattleStatus status)
    {
        var context = BattleEntity.BattleContext;
        if (status == BattleStatus.Invisible)
        {
            var relationShip = context.GetRelationship(BattleSide.Player, BattleEntity.BattleSide);
            switch (relationShip)
            {
                case BattleRelationship.Ally:
                    _renderer.DOKill(true);
                    _renderer.DOFade(0.45f, 1f);
                    break;
                case BattleRelationship.Enemy:
                    _renderer.DOKill(true);
                    _renderer.DOFade(0f, 1f);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }

    private void OnStatusDisappeared(BattleStatus status)
    {
        var context = BattleEntity.BattleContext;
        if (status == BattleStatus.Invisible)
        {
            var relationShip = context.GetRelationship(BattleSide.Player, BattleEntity.BattleSide);
            switch (relationShip)
            {
                case BattleRelationship.Ally:
                    _renderer.DOKill(true);
                    _renderer.DOFade(1, 0.7f);
                    break;
                case BattleRelationship.Enemy:
                    _renderer.DOKill(true);
                    _renderer.DOFade(1, 0.7f);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }

    private void OnEnable()
    {
        if (BattleEntity != null)
        {
            BattleEntity.DeployedBattleMap.PlacedOnMap += OnPlaced;
            BattleEntity.DeployedBattleMap.RemovedFromMap += OnRemoved;
            BattleEntity.MovedFromTo += OnMoved;
            BattleEntity.Damaged += OnDamaged;
            BattleEntity.Healed += OnHealed;
            BattleEntity.Died += OnDied;
            BattleEntity.DisposedFromBattle += OnDisposedFromBattle;
            BattleEntity.StatusHolder.StatusAppeared += OnStatusAppeared;
            BattleEntity.StatusHolder.StatusDisappeared += OnStatusDisappeared;
        }
    }

    private void OnDisable()
    {
        this.DOComplete();
        if (BattleEntity != null)
        {
            BattleEntity.DeployedBattleMap.PlacedOnMap -= OnPlaced;
            BattleEntity.DeployedBattleMap.RemovedFromMap -= OnRemoved;
            BattleEntity.MovedFromTo -= OnMoved;
            BattleEntity.Damaged -= OnDamaged;
            BattleEntity.Healed -= OnHealed;
            BattleEntity.Died -= OnDied;
            BattleEntity.DisposedFromBattle -= OnDisposedFromBattle;
            BattleEntity.StatusHolder.StatusAppeared -= OnStatusAppeared;
            BattleEntity.StatusHolder.StatusDisappeared -= OnStatusDisappeared;
            //...
        }
    }
}
