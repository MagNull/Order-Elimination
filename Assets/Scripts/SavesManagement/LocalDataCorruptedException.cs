using System;

namespace OrderElimination.SavesManagement
{
    public class LocalDataCorruptedException : Exception
    {
        private readonly string _message;

        public override string Message => _message;

        public LocalDataCorruptedException()
        {
            _message = "Local data is corrupted.";
        }

        public LocalDataCorruptedException(string message)
        {
            _message = message;
        }
    }
}
