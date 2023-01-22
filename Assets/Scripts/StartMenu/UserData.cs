using System;

namespace OrderElimination
{
    [Serializable]
    public class UserData
    {
        public string Email;
        public string Login;
        public string Password;

        public UserData(string email, string login, string password)
        {
            Email = email;
            Login = login;
            Password = password;
        }
    }
}
