using System.Collections.Generic;

namespace OrderElimination.AbilitySystem
{
    public class AbilityGameRepresentation
    {
        public readonly Dictionary<ActionPoint, int> Cost;
        public int CooldownTime;
        //AbilityTags[] Tags; //Melee, Range, Damage, ...
        //ActivationType: Manual, Automatic, Combined
    }
}
