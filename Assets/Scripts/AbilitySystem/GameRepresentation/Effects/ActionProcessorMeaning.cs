namespace OrderElimination.AbilitySystem
{
    public class ActionProcessorMeaning
    {
        public enum TargetValueType
        {
            Unknown,
            Attack,
            Heal,
            Accuracy,
        }

        public IActionProcessor ActionProcessor { get; }

        public TargetValueType TargetValue { get; set; }

        public EffectCharacter AffectionCharacter { get; set; } //good/bad
        //DescriptionFunction (one parameter function)
    }
}
