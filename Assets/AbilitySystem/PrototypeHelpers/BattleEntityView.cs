using Cysharp.Threading.Tasks;
using DefaultNamespace;
using DG.Tweening;
using OrderElimination.AbilitySystem;
using OrderElimination.AbilitySystem.Animations;
using UnityEngine;
using VContainer;

public class BattleEntityView : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer _renderer;
    [SerializeField]
    private Transform _floatingNumbersPosition;

    private BattleMapView _battleMapView;
    private IParticlesPool _particlesPool;
    private TextEmitter _textEmitter;
    private Vector3 _currentUnanimatedPosition;

    public AbilitySystemActor BattleEntity { get; private set; }
    public string Name { get; private set; }
    public Sprite BattleIcon { get; private set; }

    [Inject]
    public void Construct(BattleMapView battleMapView, IParticlesPool particlesPool, TextEmitter textEmitter)
    {
        _battleMapView = battleMapView;
        _particlesPool = particlesPool;
        _textEmitter = textEmitter;
    }

    public void Initialize(AbilitySystemActor entity, Sprite battleIcon, string name)
    {
        if (BattleEntity != null)
            return;
        BattleEntity = entity;
        if (!BattleEntity.DeployedBattleMap.Contains(BattleEntity))
            throw new System.InvalidOperationException("Enemy hasn't been located on the map yet.");
        var position = BattleEntity.Position;
        _currentUnanimatedPosition = _battleMapView.GetCell(position.x, position.y).transform.position;
        transform.position = _currentUnanimatedPosition;
        _renderer.sprite = BattleIcon = battleIcon;
        gameObject.name = Name = $"{entity.BattleSide} «{name}»";
        BattleEntity.MovedFromTo -= OnMoved;
        BattleEntity.MovedFromTo += OnMoved;
        BattleEntity.Damaged -= OnDamaged;
        BattleEntity.Damaged += OnDamaged;
        BattleEntity.Healed -= OnHealed;
        BattleEntity.Healed += OnHealed;
        BattleEntity.Died -= OnDied;
        BattleEntity.Died += OnDied;
    }

    public async UniTask Shake(float shakeX = 0.5f, float shakeY = 0.5f, float duration = 1, int vibrations = 10)
    {
        transform.DOComplete();
        await transform.DOShakePosition(duration, new Vector3(shakeX, shakeY, 0), vibrations).AsyncWaitForCompletion();
    }

    private void OnMoved(Vector2Int from, Vector2Int to)
    {
        _currentUnanimatedPosition = _battleMapView.GetCell(to.x, to.y).transform.position;
        transform.position = _currentUnanimatedPosition;
    }

    private async void OnDamaged(DealtDamageInfo damageInfo)
    {
        var strength = Mathf.InverseLerp(0, 200, damageInfo.TotalDamage);
        var shake = Mathf.Lerp(0, 0.5f, strength);
        var position = _floatingNumbersPosition.position;
        _textEmitter.Emit($"{damageInfo.TotalDamage}", Color.red, position, new Vector2(0.5f, 0.5f));
        Shake(shake, shake, 1, 10);
        //Time.timeScale = 0.25f;
        //await UniTask.Delay(2000, ignoreTimeScale: true);
        //Time.timeScale = 1f;
    }

    private void OnHealed(HealRecoveryInfo healInfo)
    {
        var position = _floatingNumbersPosition.position;
        var offset = Vector3.up * 0.25f;
        _textEmitter.Emit($"+{healInfo.RecoveredArmor}", Color.cyan, position + offset, new Vector2(0.5f, 0.5f), duration: 1);
        _textEmitter.Emit($"+{healInfo.RecoveredHealth}", Color.green, position - offset, new Vector2(0.5f, 0.5f), duration: 1);
        Shake(0, 0.07f, 1.5f, 3);
    }

    private void OnDied(AbilitySystemActor entity)
    {
        var luminosity = 0.7f;
        _renderer.DOFade(0.7f, 1).SetEase(Ease.InBounce);
        _renderer.DOColor(new Color(luminosity, luminosity, luminosity), 1);
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
