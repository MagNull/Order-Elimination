using System;

namespace OrderElimination.SavesManagement
{
    public class FailedToSaveLocalDataException : Exception
    {

        private readonly string _message;

        public override string Message => _message;

        public FailedToSaveLocalDataException()
        {
            _message = "Failed to save local data.";
        }

        public FailedToSaveLocalDataException(string message)
        {
            _message = message;
        }
    }
}
