using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;

    public IBattleObject Spawn(CellView cell)
    {
        Vector3 pos = cell.transform.position;
        GameObject obj = Instantiate(_prefab, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
        return obj.GetComponent<BattleObject>();
    }
}
