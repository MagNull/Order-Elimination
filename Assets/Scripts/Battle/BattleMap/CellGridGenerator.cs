using Sirenix.OdinInspector;
using UnityEngine;

public class CellGridGenerator : MonoBehaviour
{
    [SerializeField]
    private Transform _cellHolder;

    [SerializeField]
    private BoxCollider2D _fitCollider;

    [SerializeField]
    private CellView _cellPrefab;

    [ShowInInspector]
    public Vector2 PrefabCellSize => _cellPrefab.Size;

    [ShowInInspector]
    public Vector2 GeneratedCellSize { get; private set; }

    public CellGrid GenerateGrid(int width, int height)
    {
        var viewGrid = new CellView[width, height];
        var modelGrid = new Cell[width, height];
        var xStart = _fitCollider.bounds.min.x;
        var yStart = _fitCollider.bounds.min.y;
        var xEnd = _fitCollider.bounds.max.x;
        var yEnd = _fitCollider.bounds.max.y;
        GeneratedCellSize = new Vector2((xEnd - xStart) / width, (yEnd - yStart) / height);
        var scaler = GeneratedCellSize / PrefabCellSize;
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var cellView = Instantiate(_cellPrefab, _cellHolder);
                cellView.transform.position = new Vector3(
                    xStart + i * GeneratedCellSize.x + GeneratedCellSize.x / 2,
                    yStart + j * GeneratedCellSize.y + GeneratedCellSize.y / 2,
                    0);
                cellView.transform.localScale *= scaler;

                var cellModel = new Cell();
                cellView.BindModel(cellModel);

                viewGrid[i, j] = cellView;
                modelGrid[i, j] = cellModel;
            }
        }

        return new CellGrid(viewGrid, modelGrid);
    }
}