using DG.Tweening;
using UnityEngine;

namespace RoguelikeMap.Panels
{
    public class EventPanel : IPanel
    {
        //TODO(coder): add loot to player inventory after create inventory system
        public void Accept()
        {
            Debug.Log("Accept on event");
            Close();
        }

        public void Decline()
        {
            Debug.Log("Decline on event");
            Close();
        }
    }
}