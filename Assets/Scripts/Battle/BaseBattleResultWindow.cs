using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBattleResultWindow : MonoBehaviour
{
    public virtual void View()
    {
        Instantiate(gameObject);
    }
}
