using OrderElimination.BattleMap;
using UnityEngine;

public class CellGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform _parent;
    [SerializeField]
    private CellView _cellPrefab;
    [SerializeField]
    private float _spaceBetweenCells;

    public CellGrid GenerateGrid(int width, int height)
    {
        CellView[,] viewGrid = new CellView[width, height];
        CellModel[,] modelGrid = new CellModel[width, height];

        float x = _cellPrefab.transform.localScale.x;
        float y = _cellPrefab.transform.localScale.y;

        float xStart = -(float) width / 2;
        float yStart = -(float) height / 2;

        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                CellModel currentModel = new CellModel();
                CellView currentObject = Instantiate(_cellPrefab,
                    new Vector3(xStart + i * (x + _spaceBetweenCells) + _parent.position.x,
                        yStart + j * (y + _spaceBetweenCells) + _parent.position.y, 0),
                    Quaternion.identity,
                    _parent);

                currentObject.BindModel(currentModel);

                viewGrid[i, j] = currentObject;
                modelGrid[i, j] = currentModel;
            }
        }

        return new CellGrid(viewGrid, modelGrid);
    }
}