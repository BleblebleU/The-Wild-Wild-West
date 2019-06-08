using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyPathfindManager : MonoBehaviour
{
    private GameObject enemyColMapObj;

    private static Tilemap levelEnemyColMap;
    private static bool[,] mapData;
    private static EnmyTileColData colmapData;

    private PathFind.Grid grid;

    private void Awake()
    {
        enemyColMapObj = GameObject.FindGameObjectWithTag("EnemyColMap");
        levelEnemyColMap = enemyColMapObj.GetComponent<Tilemap>();
        colmapData = enemyColMapObj.GetComponent<EnmyTileColData>();
    }

    // Start is called before the first frame update
    void Start()
    {
        mapData = new bool[colmapData.maxLength, colmapData.maxWidth];
        Vector2Int mapOrigin = colmapData.minOrigin;
        //Debug.Log(levelEnemyColMap.HasTile(new Vector3Int(9, 11, 0)));
        for (int i = mapOrigin.x; i < colmapData.maxLength; i++)
        {
            for (int j = mapOrigin.y; j < colmapData.maxWidth; j++)
            {
                if (!levelEnemyColMap.HasTile(new Vector3Int(i, j, 0)))
                {
                    mapData[i, j] = true;
                }
            }
        }
        grid = new PathFind.Grid(mapData);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public Vector3[] GetPath(Vector3 start, Vector3 target)
    {
        Vector3Int startPos = levelEnemyColMap.WorldToCell(start);
        Vector3Int targetPos = levelEnemyColMap.WorldToCell(target);
        PathFind.Point _from = new PathFind.Point(Mathf.Abs(startPos.x), Mathf.Abs(startPos.y));
        PathFind.Point _to = new PathFind.Point(Mathf.Abs(targetPos.x), Mathf.Abs(targetPos.y));

        // get path
        // path will either be a list of Points (x, y), or an empty list if no path is found.
        return TileToWorldList(PathFind.Pathfinding.FindPath(grid, _from, _to, PathFind.Pathfinding.DistanceType.Manhattan));
    }

    private Vector3[] TileToWorldList(List<PathFind.Point> points)
    {
        Vector3[] returnPts = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            returnPts[i] = levelEnemyColMap.GetCellCenterWorld(new Vector3Int(points[i].x, points[i].y, 0));
        }
        return returnPts;
    }
}
