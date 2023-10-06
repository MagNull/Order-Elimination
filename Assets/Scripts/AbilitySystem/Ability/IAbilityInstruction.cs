using Cysharp.Threading.Tasks;

namespace OrderElimination.AbilitySystem
{
    //Not utilized yet. Must replace AbilityInstructions
    public interface IAbilityInstruction
    {
        public UniTask Execute(AbilityExecutionContext executionContext);
    }
}
