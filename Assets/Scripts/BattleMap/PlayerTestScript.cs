using UnityEngine;

// ��������� ����� ��� ���������
// - �������� ������� ��� � ����������� �� ���������� ������� �������
public class PlayerTestScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _view;
    [SerializeField] private Sprite _enemySprite;

    public void SetSide(BattleObjectType type)
    {
        _view.sprite = type == BattleObjectType.Ally ? _view.sprite : _enemySprite;
    }
}
