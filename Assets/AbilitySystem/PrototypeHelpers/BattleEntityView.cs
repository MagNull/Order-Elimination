using Cysharp.Threading.Tasks;
using DG.Tweening;
using OrderElimination;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using OrderElimination.Infrastructure;
using OrderElimination.Utils;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public class BattleEntityView : SerializedMonoBehaviour
{
    [HideInInspector, SerializeField]
    private float _iconTargetSize = 1.9f;

    [Title("Components")]
    [SerializeField]
    private SpriteRenderer _renderer;

    //[SerializeField]
    //private Transform _floatingNumbersPosition;

    [SerializeField]
    private Transform _appliedEffectsPosition;

    [Title("Parameters")]
    [SerializeField]
    private bool _showFloatingNumbers = true;

    [ShowIf(nameof(_showFloatingNumbers))]
    [ShowInInspector, OdinSerialize]
    private TextEmitterContext _floatingNumbersPreset = TextEmitterContext.Default;

    [ShowIf(nameof(_showFloatingNumbers))]
    [SerializeField]
    private bool _roundFloatingNumbers = true;

    [ShowIf("@" + nameof(_showFloatingNumbers))]
    [EnableIf(nameof(_roundFloatingNumbers))]
    [SerializeField]
    private RoundingOption _roundingMode;


    private BattleMapView _battleMapView;
    private IParticlesPool _particlesPool;
    private TextEmitter _textEmitter;
    private SpriteEmitter _spriteEmitter;
    private float _damageCash;
    private float _healthCash;
    private float _armorCash;
    private bool _isDisposed;

    private static float TimeGapToSumValues { get; set; } = 0.01f;

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
                float maxBoundsSide = Mathf.Max(bounds.size.x, bounds.size.y);
                _renderer.transform.localScale = Vector3.one * IconTargetSize / maxBoundsSide;
            }
        }
    }
    public GameObject CustomModel { get; set; }

    public bool IsVisibleToPlayer
    {
        get
        {
            if (BattleEntity.StatusHolder.HasStatus(BattleStatus.Invisible))
            {
                var relationShip = BattleEntity.BattleContext.GetRelationship(
                    BattleSide.Player, BattleEntity.BattleSide);
                if (relationShip != BattleRelationship.Ally)
                    return false;
            }
            return true;
        }
    }

    [Inject]
    public void Construct(
        BattleMapView battleMapView, IParticlesPool particlesPool, TextEmitter textEmitter, SpriteEmitter spriteEmitter)
    {
        _battleMapView = battleMapView;
        _particlesPool = particlesPool;
        _textEmitter = textEmitter;
        _spriteEmitter = spriteEmitter;
    }

    public void Initialize(AbilitySystemActor entity, string name, Sprite battleIcon, GameObject customModel = null)
    {
        if (BattleEntity != null)
            return;

        BattleEntity = entity;
        UnsubscribeFromEntityEvents(BattleEntity);
        SubscribeOnEntityEvents(BattleEntity);

        BattleIcon = battleIcon;
        gameObject.name = Name = $"{entity.BattleSide} «{name}»";
        if (customModel != null)
        {
            CustomModel = Instantiate(customModel, transform);
        }
        if (BattleEntity.DeployedBattleMap.ContainsEntity(BattleEntity))
        {
            throw new InvalidOperationException("Initialize EntityView first and place entity on map after.");
            var gamePosition = BattleEntity.Position;
            transform.position = _battleMapView.GetCell(gamePosition.x, gamePosition.y).transform.position;
        }
    }

    public void Highlight(Color highlightColor)
        => Highlight(highlightColor, 0.1f, 0.2f, 0.3f);

    public void Highlight(Color highlightColor, float highlightTime, float duration, float fadeTime)
    {
        _renderer.DOComplete();
        var initialColor = _renderer.color;
        DOTween.Sequence(_renderer)
            .Append(_renderer.DOColor(highlightColor, highlightTime))
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
        if (!IsVisibleToPlayer) return;
        if (_damageCash > 0)//waiting
        {
            _damageCash += damageInfo.TotalDealtDamage;
            return;
        }
        else//no cash -> start cashing
        {
            _damageCash += damageInfo.TotalDealtDamage;
            await UniTask.Delay(Mathf.RoundToInt(TimeGapToSumValues * 1000), true);
        }
        var damageValue = _damageCash;
        _damageCash = 0;

        var strength = Mathf.InverseLerp(0, 200, damageValue);
        var shake = Mathf.Lerp(0, 0.5f, strength);
        float roundedDamage = MathExtensions.Round(damageValue, _roundingMode);
        if (_roundFloatingNumbers)
            damageValue = roundedDamage;
        var damageText = _floatingNumbersPreset;
        damageText.Text = $"{damageValue}";
        damageText.TextColor = Color.red;
        damageText.Origin += transform.position;
        if (_showFloatingNumbers)
            _textEmitter.Emit(damageText);
                //_textEmitter.Emit($"{damageValue}", Color.red, position, new Vector2(0.5f, 0.5f));
        if (!BattleEntity.IsDisposedFromBattle)
            Shake(shake, shake, 1, 10);
    }

    private async void OnHealed(DealtRecoveryInfo healInfo)
    {
        if (!IsVisibleToPlayer) return;
        if (_healthCash > 0 || _armorCash > 0)//waiting
        {
            _healthCash += healInfo.TotalHealthRecovery;
            _armorCash += healInfo.TotalArmorRecovery;
            return;
        }
        else//no cash -> start cashing
        {
            _healthCash += healInfo.TotalHealthRecovery;
            _armorCash += healInfo.TotalArmorRecovery;
            await UniTask.Delay(Mathf.RoundToInt(TimeGapToSumValues * 1000), true);
        }
        var healthValue = _healthCash;
        var armorValue = _armorCash;
        _healthCash = 0;
        _armorCash = 0;

        var statsOffset = Vector3.up * 0.25f;
        if (_showFloatingNumbers)
        {
            if (_roundFloatingNumbers)
            {
                armorValue = MathExtensions.Round(armorValue, _roundingMode);
                healthValue = MathExtensions.Round(healthValue, _roundingMode);
            }
            var healthText = _floatingNumbersPreset;
            healthText.Text = $"+{healthValue}";
            healthText.TextColor = Color.green;
            healthText.Origin += transform.position - statsOffset;

            var armorText = _floatingNumbersPreset;
            armorText.Text = $"+{armorValue}";
            armorText.TextColor = Color.cyan;
            armorText.Origin += transform.position + statsOffset;

            _textEmitter.Emit(healthText);
            _textEmitter.Emit(armorText);
        }
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
        _isDisposed = true;
        var luminosity = 0.1f;
        //_renderer.DOFade(0.7f, 1).SetEase(Ease.InBounce);
        var tasks = new List<Task>
        {
            _renderer.DOColor(new Color(luminosity, luminosity, luminosity), 0.4f).AsyncWaitForCompletion(),
            _renderer.DOFade(0, 0.3f).SetDelay(0.4f).SetEase(Ease.OutBounce).AsyncWaitForCompletion()
        };
        await Task.WhenAll(tasks);
        _renderer.DOComplete();
        _renderer.enabled = false;
        if (CustomModel != null)
            CustomModel.SetActive(false);
        //gameObject.SetActive(false);
    }

    private void OnStatusAppeared(BattleStatus status)
    {
        if (_isDisposed) return;
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
                case BattleRelationship.Neutral:
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
        if (_isDisposed) return;
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
                case BattleRelationship.Neutral:
                    _renderer.DOKill(true);
                    _renderer.DOFade(1, 0.7f);
                    break;
                default:
                    throw new System.NotImplementedException();
            }
        }
    }

    private void OnEffectAdded(BattleEffect effect)
    {
        var effectView = effect.EffectData.View;
        if (effectView.IsHidden)
            return;
        if (!IsVisibleToPlayer)
            return;
        //TODO: multiple effects at the same time case
        _spriteEmitter.Emit(effectView.Icon, _appliedEffectsPosition.position);
    }

    private void SubscribeOnEntityEvents(AbilitySystemActor entity)
    {
        entity.DeployedBattleMap.PlacedOnMap += OnPlaced;
        entity.DeployedBattleMap.RemovedFromMap += OnRemoved;
        entity.MovedFromTo += OnMoved;
        entity.Damaged += OnDamaged;
        entity.Healed += OnHealed;
        entity.StatusHolder.StatusAppeared += OnStatusAppeared;
        entity.StatusHolder.StatusDisappeared += OnStatusDisappeared;
        entity.EffectAdded += OnEffectAdded;
        entity.Died += OnDied;
        entity.DisposedFromBattle += OnDisposedFromBattle;
    }

    private void UnsubscribeFromEntityEvents(AbilitySystemActor entity)
    {
        entity.DeployedBattleMap.PlacedOnMap -= OnPlaced;
        entity.DeployedBattleMap.RemovedFromMap -= OnRemoved;
        entity.MovedFromTo -= OnMoved;
        entity.Damaged -= OnDamaged;
        entity.Healed -= OnHealed;
        entity.StatusHolder.StatusAppeared -= OnStatusAppeared;
        entity.StatusHolder.StatusDisappeared -= OnStatusDisappeared;
        entity.EffectAdded -= OnEffectAdded;
        entity.Died -= OnDied;
        entity.DisposedFromBattle -= OnDisposedFromBattle;
    }

    private void OnEnable()
    {
        if (BattleEntity != null)
        {
            SubscribeOnEntityEvents(BattleEntity);
        }
    }

    private void OnDisable()
    {
        if (BattleEntity != null)
        {
            UnsubscribeFromEntityEvents(BattleEntity);
        }
    }
}
