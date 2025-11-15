using UnityEngine;

public class GridSpawner : MonoBehaviour
{
    [SerializeField] Vector2Int gridWidth = new Vector2Int(40,20);
    [SerializeField] int gridTileSize = 2;

    [SerializeField] TransformST gridObj;

    [SerializeField] bool spawn;

    void Awake()
    {
        SpawnGrid();
    }

    void OnValidate()
    {
        if (spawn)
        {
            spawn = false;
            SpawnGrid();
        }
    }

    void SpawnGrid()
    {
        for (int i = -gridWidth.x/2; i < gridWidth.x/2; i++)
        {
            for (int j = -gridWidth.y/2; j < gridWidth.y/2; j++)
            {
                TransformST thing = Instantiate(gridObj, new Vector3(gridTileSize*i,gridTileSize*j,0), Quaternion.identity);
                thing.transform.SetParent(this.transform);
            }
        }
    }
}
