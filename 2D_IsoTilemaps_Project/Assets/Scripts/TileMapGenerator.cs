using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TileMapGenerator : MonoBehaviour
{
    public const int length = 30;
    public const int width = 30;

    static Tilemap baseMap;
    public TileBase baseTile;

    public static bool[,] mapData = new bool[length, width];

    private PathFind.Grid grid;

    private void Awake()
    {
        baseMap = GetComponent<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                baseMap.SetTile(new Vector3Int(i, j, 0), baseTile);
                mapData[i, j] = true;
            }
        }

        //Debug.Log(baseMap.HasTile(new Vector3Int(0, 0, 0)));
        //Debug.Log(baseMap.HasTile(new Vector3Int(-1, -1, 0)));
        grid = new PathFind.Grid(mapData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3[] GetPath(Vector3 start, Vector3 target)
    {
        Vector3Int startPos = baseMap.WorldToCell(start);
        Vector3Int targetPos = baseMap.WorldToCell(target);
        PathFind.Point _from = new PathFind.Point(Mathf.Abs(startPos.x), Mathf.Abs(startPos.y));
        PathFind.Point _to = new PathFind.Point(Mathf.Abs(targetPos.x), Mathf.Abs(targetPos.y));

        // get path
        // path will either be a list of Points (x, y), or an empty list if no path is found.
        return TileToWorldList(PathFind.Pathfinding.FindPath(grid, _from, _to));
    }

    private Vector3[] TileToWorldList(List<PathFind.Point> points)
    {
        Vector3[] returnPts = new Vector3[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            returnPts[i] = baseMap.CellToWorld(new Vector3Int(points[i].x, points[i].y, 0));
        }
        return returnPts;
    }
}
