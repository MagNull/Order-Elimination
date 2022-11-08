using System;

public interface IDamageable
{
    event Action<int> Damaged;
    void TakeDamage(int damage, int accuracy);
}