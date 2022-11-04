using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��������� ����� ��� ���������
// - �������� ������� ��� � ����������� �� ���������� ������� �������
public class PlayerTestScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sideCircle;

    public void SetSide(BattleObjectSide side)
    {
        sideCircle.color = side == BattleObjectSide.Player ? Color.green : Color.red;
    }
}
