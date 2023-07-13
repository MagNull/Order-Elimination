using System;

namespace OrderElimination.SavesManagement
{
    public class FailedToLoadLocalDataException : Exception
    {
        private readonly string _message;

        public override string Message => _message;

        public FailedToLoadLocalDataException()
        {
            _message = "Failed to load local data.";
        }

        public FailedToLoadLocalDataException(string message)
        {
            _message = message;
        }
    }
}
