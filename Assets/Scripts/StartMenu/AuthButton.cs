using UnityEngine;
using UnityEngine.EventSystems;

namespace OrderElimination
{
    public class AuthButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        private ButtonType _buttonType = ButtonType.None;

        private enum ButtonType
        {
            None = 0,
            SignIn = 1,
            SignUp = 2
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_buttonType == ButtonType.SignIn)
                AuthManager.OnUserSignIn?.Invoke();
            else if(_buttonType == ButtonType.SignUp)
                AuthManager.OnUserSignUp?.Invoke();
        }
    }
}