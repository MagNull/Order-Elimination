using System;
using UnityEngine;

namespace OrderElimination.Start
{
    public class Saves : MonoBehaviour
    {
        public static event Action<bool> ExitSavesWindow; 

        public void ExitClicked()
        {
            ExitSavesWindow?.Invoke(true);
            gameObject.SetActive(false);
        }
    }
}