using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��������� ����� ��� ���������
// - �������� ������� ��� � ����������� �� ���������� ������� �������
public class PlayerTestScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sideCircle;

    public void SetSide(CharacterSide side)
    {
        sideCircle.color = side == CharacterSide.Player ? Color.green : Color.red;
    }
}
