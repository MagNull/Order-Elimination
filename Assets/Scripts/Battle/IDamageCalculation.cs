namespace OrderElimination.Battle
{
    public interface IDamageCalculation
    {
        public (int healtDamage, int armorDamage) CalculateDamage(int attack, int armor, int accuracy, int evasion);
    }
}