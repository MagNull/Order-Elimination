using System;
using UnityEngine;

public interface IBattleObjectView
{
    public event Action<IBattleObjectView> Disabled;

    public GameObject GameObject { get; }

    public IBattleObject Model { get; }
    public void Disable();
    public void SetImage(Sprite image);

    public void ShowAccuracy(float probability);
    public void HideAccuracy();

    public void OnDamaged(TakeDamageInfo damageInfo);
}