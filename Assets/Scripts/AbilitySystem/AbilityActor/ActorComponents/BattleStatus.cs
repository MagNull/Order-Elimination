namespace OrderElimination.AbilitySystem
{
    public enum BattleStatus
    {
        Invisible = 1,
        CantMove = 2,
        Invulnerable = 4, // - Cant be damaged
        FreeMovement = 8, // - can move freely //NOT USED YET
        ActiveAbilitiesDisabled = 16,
        PassiveAbilitiesDisabled = 32,
        //CantDie = 64, // - TakesDamage but health cant get lower than 1
    }
}
