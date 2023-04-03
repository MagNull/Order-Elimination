using UnityEngine;

// ¬ременный класс дл€ прототипа
// - »змен€ет внешний вид в зависимости от занимаемой фигурой стороны
public class PlayerTestScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _view;
    [SerializeField] private Sprite _enemySprite;

    public void SetSide(BattleObjectType type)
    {
        _view.sprite = type == BattleObjectType.Ally ? _view.sprite : _enemySprite;
    }
}
