using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ¬ременный класс дл€ прототипа
// - »змен€ет внешний вид в зависимости от занимаемой фигурой стороны
public class PlayerTestScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sideCircle;

    public void SetSide(BattleObjectSide side)
    {
        sideCircle.color = side == BattleObjectSide.Player ? Color.green : Color.red;
    }
}
