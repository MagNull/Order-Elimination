using UnityEngine;

namespace RoguelikeMap.Panels
{
    public class IPanel : MonoBehaviour
    {
        protected static float StartPosition { get; private set; } = 3000f;
        protected const float EndPositionHalfWindow = 1500f;
        protected const float EndPositionFullScreenWindow = 970f;
        
        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }
    }
}