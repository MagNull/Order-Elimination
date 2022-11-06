using OrderElimination.BattleMap;
using UnityEngine;

// Временный класс для прототипа
public class CellGridGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject _parent;
    [SerializeField]
    private CellView _cellPrefab;

    public CellView[,] GenerateGrid(int width, int height)
    {
        CellView[,] cellGrid = new CellView[width, height];

        float x = _cellPrefab.transform.localScale.x;
        float y = _cellPrefab.transform.localScale.y;

        float xStart = -(float)width / 2;
        float yStart = -(float)height / 2;

        float delta = 0.4f;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
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