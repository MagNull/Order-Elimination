using UnityEngine;

namespace RoguelikeMap.Panels
{
    public class Panel : MonoBehaviour
    {
        public void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}