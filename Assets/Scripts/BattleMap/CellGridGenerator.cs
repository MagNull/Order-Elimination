using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGridGenerator : MonoBehaviour
{
    [SerializeField] public GameObject _parent;
    [SerializeField] public GameObject _cellPrefab;

    public CellView[,] GenerateGrid()
    {
        CellView[,] cellGrid = new CellView[8, 8];

        float x = _cellPrefab.transform.localScale.x;
        float y = _cellPrefab.transform.localScale.y;

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                GameObject currentObject = Instantiate(_cellPrefab, new Vector3(i * (x + 1), j * (y + 1), 0), Quaternion.identity);
                cellGrid[i, j] = currentObject.GetComponent<CellView>();
            }
        }
        return cellGrid;
    }
}
