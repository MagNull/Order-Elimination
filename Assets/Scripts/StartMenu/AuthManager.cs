using System.Collections.Generic;
using UnityEngine;
using Proyecto26;
using System;
using Newtonsoft.Json;
using TMPro;

namespace OrderElimination
{
    public class AuthManager : MonoBehaviour
    {
        public GameObject loginForm;
        public GameObject registerForm;

        [Header("Login / Sign In")] 
        public TMP_InputField signInEmail;
        public TMP_InputField signInPassword;

        [Header("Register / Sign Up")] 
        public TMP_InputField signUpEmail;
        public TMP_InputField signUpLogin;
        public TMP_InputField signUpPassword;
        public TMP_InputField signUpConfirmPassword;

        [Space(10)] 
        public TMP_Text signInMessage;
        public TMP_Text signUpMessage;

        private string AUTH_KEY = "AIzaSyCU9is2eVObujGy9-FqtqO6IHagePunw3Y";

        public static Action OnUserSignIn;
        public static Action OnUserSignUp;
        public static event Action<string> OnUserLogin;

        private void Start()
        {
            OnUserSignUp += SignUp;
            OnUserSignIn += SignIn;
        }

        private void OnDestroy()
        {
            OnUserSignUp -= SignUp;
            OnUserSignIn -= SignIn;
        }

        public void GetUserByEmail(string email)
        {
            Database.FindUserByEmail(email, GetUserByEmail);
        }

        public void GetUserByEmail(RequestException exception, ResponseHelper helper)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, UserData>>(helper.Text);

                foreach (var keyValuePair in dict)
                {
                    loginForm.SetActive(false);
                    OnUserLogin?.Invoke(keyValuePair.Value.Login);
                    break;
                }
            }
            catch (Exception e)
            {
                Debug.Log("User data not loaded");
            }
        }

        private void SignIn()
        {
            var email = signInEmail.text.ToLower();
            var password = signInPassword.text;
            SignIn(email, password);
        }

        private void SignIn(string email, string password)
        {
            signInMessage.text = "Search Account...";
            signInMessage.color = Color.white;

            var data = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

            RestClient.Post<AuthData>(
                $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={AUTH_KEY}", data,
                SignInCallback);
        }

        private void SignInCallback(RequestException exception, ResponseHelper helper, AuthData data)
        {
            try
            {
                signInMessage.text = "Account Initialized";
                signInMessage.color = Color.green;
                GetUserByEmail(data.email);
            }
            catch (Exception e)
            {
                signInMessage.text = "Неверный логин или пароль";
                signInMessage.color = Color.red;
            }
        }

        private void SignUp()
        {
            var email = signUpEmail.text.ToLower();
            var login = signUpLogin.text;
            var password = signUpPassword.text;

            var isEmailEmpty = string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email);
            var isPasswordEmpty = string.IsNullOrEmpty(password) || string.IsNullOrWhiteSpace(password);

            signUpMessage.text = "Waiting, you'r account creating...";
            signUpMessage.color = Color.white;

            if (!string.IsNullOrEmpty(login))
            {
                Database.GetUserByLogin(login, GetUserByLoginCallback);
            }
        }

        private void GetUserByLoginCallback(RequestException exception, ResponseHelper helper, UserData userData)
        {
            if (userData == null)
            {
                if (signUpPassword.text.Length >= 6 && signUpConfirmPassword.text.Length >= 6)
                {
                    if (signUpPassword.text == signUpConfirmPassword.text)
                    {
                        var data = "{\"email\":\"" + signUpEmail.text + "\",\"password\":\"" + signUpPassword.text + "\",\"returnSecureToken\":true}";
                        RestClient.Post<AuthData>(
                            $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={AUTH_KEY}", data,
                            SignUpCallback);
                    }
                    else if (signUpPassword.text != signUpConfirmPassword.text)
                    {
                        signUpMessage.text = "Password not equals";
                        signUpMessage.color = Color.red;
                    }
                }
                else if (signUpPassword.text.Length < 6 || signUpConfirmPassword.text.Length < 6)
                {
                    signUpMessage.text = "Password not enough lenght!";
                    signUpMessage.color = Color.red;
                }
                else
                {
                    signUpMessage.text = "Account with same login have in system";
                    signUpMessage.color = Color.red;
                }
            }
            else
            {
                signUpMessage.text = "Account with same mail/login have in system";
                signUpMessage.color = Color.red;
            }
        }

        private void SignUpCallback(RequestException exception, ResponseHelper helper, AuthData data)
        {
            try
            {
                var email = signUpEmail.text.ToLower();
                var login = signUpLogin.text;
                var password = signUpPassword.text;
                
                signUpMessage.text = "Account created";
                signUpMessage.color = Color.green;

                var userData = new UserData(email, login, password);
                Database.SendToDatabase(userData, login);
                registerForm.SetActive(false);
                loginForm.SetActive(true);
            }
            catch (Exception e)
            {
                signUpMessage.text = e.Message;
                signUpMessage.color = Color.red;
            }
        }

        public void LogOut()
        {
            if (!PlayerPrefs.HasKey("Id"))
                PlayerPrefs.DeleteKey("Id");
            loginForm.SetActive(true);
        }
    }

    [Serializable]
    public class AuthData
    {
        public string localId;
        public string email;
    }
}


