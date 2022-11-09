using UnityEngine;

// ¬ременный класс дл€ прототипа
// - »змен€ет внешний вид в зависимости от занимаемой фигурой стороны
public class PlayerTestScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _view;
    [SerializeField] private Sprite _enemySprite;

    public void SetSide(BattleObjectSide side)
    {
        _view.sprite = side == BattleObjectSide.Player ? _view.sprite : _enemySprite;
    }
}
