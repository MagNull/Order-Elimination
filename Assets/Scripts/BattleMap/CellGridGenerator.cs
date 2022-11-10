using OrderElimination.BattleMap;
using UnityEngine;

// Временный класс для прототипа
public class CellGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform _parent;
    [SerializeField]
    private CellView _cellPrefab;
    [SerializeField]
    private float _spaceBetweenCells;

    public CellView[,] GenerateGrid(int width, int height)
    {
        CellView[,] cellGrid = new CellView[width, height];

        float x = _cellPrefab.transform.localScale.x;
        float y = _cellPrefab.transform.localScale.y;

        float xStart = -(float) width / 2;
        float yStart = -(float) height / 2;


        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                CellView currentObject = Instantiate(_cellPrefab,
                    new Vector3(xStart + i * (x + _spaceBetweenCells) + _parent.position.x,
                        yStart + j * (y + _spaceBetweenCells) + _parent.position.y, 0),
                    Quaternion.identity,
                    _parent);
                currentObject.SetObject(new NullBattleObject());
                cellGrid[i, j] = currentObject;
            }
        }

        return cellGrid;
    }
}