using System;

namespace OrderElimination.AbilitySystem
{
    public class NotEnoughDataArgumentException : ArgumentException
    {
        public override string Message => "Argument does not contain required data.";
    }
}
