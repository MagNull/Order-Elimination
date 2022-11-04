using OrderElimination.BattleMap;
using UnityEngine;

// Временный класс для прототипа
public class CellGridGenerator : MonoBehaviour
{
    [SerializeField]
    public GameObject _parent;
    [SerializeField]
    public CellView _cellPrefab;

    public CellView[,] GenerateGrid()
    {
        CellView[,] cellGrid = new CellView[8, 8];

        float x = _cellPrefab.transform.localScale.x;
        float y = _cellPrefab.transform.localScale.y;

        float xStart = -4;
        float yStart = -4;

        float delta = 0.4f;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                CellView currentObject = Instantiate(_cellPrefab,
                    new Vector3(xStart + i * (x + delta), yStart + j * (y + delta), 0), Quaternion.identity);
                currentObject.SetObject(new NullBattleObject());
                currentObject.transform.parent = _parent.transform;
                cellGrid[i, j] = currentObject;
            }
        }

        return cellGrid;
    }
}