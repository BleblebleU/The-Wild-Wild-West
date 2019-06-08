using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

enum Direction
{
    left,
    right,
    down,
    up,
    stay
}

public class TestLevelGeneration : MonoBehaviour
{
    //MUST ALWASY BE A SQUARE
    public const int length = 30;
    public const int width = 30;

    static Tilemap baseMap;
    public TileBase baseTile;

    public static bool[,] mapData = new bool[length, width];

    private PathFind.Grid grid;

    private int min = 9;
    private int max = 21;

    private void Awake()
    {
        baseMap = GetComponent<Tilemap>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 0; i < length; i++)
        //{
        //    for (int j = 0; j < width; j++)
        //    {
        //        baseMap.SetTile(new Vector3Int(i, j, 0), baseTile);
        //        mapData[i, j] = true;
        //    }
        //}

        Walker(ref mapData, length, width, 100);
        for (int i = 0; i < length; i++)
        {
            baseMap.SetTile(new Vector3Int(i, 0, 0), baseTile);
            mapData[i, 0] = true;
            baseMap.SetTile(new Vector3Int(i, width - 1, 0), baseTile);
            mapData[i, width - 1] = true;
        }
        for (int i = 0; i < width; i++)
        {
            baseMap.SetTile(new Vector3Int(length - 1, i, 0), baseTile);
            mapData[length - 1, i] = true;
            baseMap.SetTile(new Vector3Int(0, i, 0), baseTile);
            mapData[0, i] = true;
        }

        for (int i = 14; i < 18; i++)
        {
            for (int j = 0; j < width; j++)
            {
                baseMap.SetTile(new Vector3Int(i , j, 0), baseTile);
                mapData[i, j] = true;
                baseMap.SetTile(new Vector3Int(j, i, 0), baseTile);
                mapData[j, i] = true;
            }
        }

        for (int i = 0; i < width; i++)
        {
            baseMap.SetTile(new Vector3Int(min, i, 0), baseTile);
            mapData[min, i] = true;
            baseMap.SetTile(new Vector3Int(max, i, 0), baseTile);
            mapData[max, i] = true;

            baseMap.SetTile(new Vector3Int(i, min, 0), baseTile);
            mapData[i, min] = true;
            baseMap.SetTile(new Vector3Int(i, max, 0), baseTile);
            mapData[i, max] = true;
        }
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

    private void Walker(ref bool[,] mapData, int length, int width, int numSteps)
    {
        Vector2Int walkerDropPoint = new Vector2Int(max, min);
        Vector2Int currentWalkerPosition = walkerDropPoint;
        Vector4 mM = minMax(walkerDropPoint.x, walkerDropPoint.y);
        Direction curDir = Direction.stay;

        for (int i = 0; i < numSteps; i++)
        {
            WalkWalker(ref curDir, ref currentWalkerPosition, (int)mM.x, (int)mM.y, (int)mM.z, (int)mM.w, out bool notValid);
            //Debug.Log(currentWalkerPosition);
            baseMap.SetTile(new Vector3Int(currentWalkerPosition.x, currentWalkerPosition.y, 0), baseTile);
            if(!notValid)
                mapData[currentWalkerPosition.x, currentWalkerPosition.y] = true;
        }
        walkerDropPoint = new Vector2Int(min, max);
        currentWalkerPosition = walkerDropPoint;
        mM = minMax(walkerDropPoint.x, walkerDropPoint.y);
        curDir = Direction.stay;

        for (int i = 0; i < numSteps; i++)
        {
            WalkWalker(ref curDir, ref currentWalkerPosition, (int)mM.x, (int)mM.y, (int)mM.z, (int)mM.w, out bool notValid);
            //Debug.Log(currentWalkerPosition);
            baseMap.SetTile(new Vector3Int(currentWalkerPosition.x, currentWalkerPosition.y, 0), baseTile);
            if (!notValid)
                mapData[currentWalkerPosition.x, currentWalkerPosition.y] = true;
        }

        walkerDropPoint = new Vector2Int(min, min);
        currentWalkerPosition = walkerDropPoint;
        mM = minMax(walkerDropPoint.x, walkerDropPoint.y);
        curDir = Direction.stay;
        
        for (int i = 0; i < numSteps; i++)
        {
            WalkWalker(ref curDir, ref currentWalkerPosition, (int)mM.x, (int)mM.y, (int)mM.z, (int)mM.w, out bool notValid);
            //Debug.Log(currentWalkerPosition);
            baseMap.SetTile(new Vector3Int(currentWalkerPosition.x, currentWalkerPosition.y, 0), baseTile);
            if (!notValid)
                mapData[currentWalkerPosition.x, currentWalkerPosition.y] = true;
        }

        walkerDropPoint = new Vector2Int(max, max);
        currentWalkerPosition = walkerDropPoint;
        mM = minMax(walkerDropPoint.x, walkerDropPoint.y);
        curDir = Direction.stay;

        for (int i = 0; i < numSteps; i++)
        {
            WalkWalker(ref curDir, ref currentWalkerPosition, (int)mM.x, (int)mM.y, (int)mM.z, (int)mM.w, out bool notValid);
            //Debug.Log(currentWalkerPosition);
            baseMap.SetTile(new Vector3Int(currentWalkerPosition.x, currentWalkerPosition.y, 0), baseTile);
            if (!notValid)
                mapData[currentWalkerPosition.x, currentWalkerPosition.y] = true;
        }

        walkerDropPoint = new Vector2Int(length / 2, width / 2);
        currentWalkerPosition = walkerDropPoint;
        mM = minMax(walkerDropPoint.x, walkerDropPoint.y);
        curDir = Direction.stay;

        for (int i = 0; i < numSteps; i++)
        {
            WalkWalker(ref curDir, ref currentWalkerPosition, (int)mM.x, (int)mM.y, (int)mM.z, (int)mM.w, out bool notValid);
            //Debug.Log(currentWalkerPosition);
            baseMap.SetTile(new Vector3Int(currentWalkerPosition.x, currentWalkerPosition.y, 0), baseTile);
            if (!notValid)
                mapData[currentWalkerPosition.x, currentWalkerPosition.y] = true;
        }
    }

    private void UpdateWalkerPosition(ref Vector2Int walkerPos, Direction dir)
    {
        if(dir == Direction.down)
        {
            walkerPos = new Vector2Int(walkerPos.x - 1, walkerPos.y);
        }
        else if (dir == Direction.up)
        {
            walkerPos = new Vector2Int(walkerPos.x + 1, walkerPos.y);
        }
        else if (dir == Direction.left)
        {
            walkerPos = new Vector2Int(walkerPos.x, walkerPos.y + 1);
        }
        else if (dir == Direction.right)
        {
            walkerPos = new Vector2Int(walkerPos.x, walkerPos.y - 1);
        }
    }

    private void WalkWalker(ref Direction currDir, ref Vector2Int walkerPos, int minX, int maxX, int minY, int maxY, out bool notValid)
    {
        int randDir = 4;
        Vector2Int startPos = walkerPos;
        Direction randDirDir = DirectionOfIndex(randDir);
        Vector2Int copyWalker;
        int repeated = 0;
        int repeatR = 0;
        do
        {
            if(repeated > 5)
            {
                walkerPos = startPos;
                repeatR++;
            }
            do{
                randDir = Random.Range(0, 4);
                randDirDir = DirectionOfIndex(randDir);
            } while (randDirDir == currDir);
            copyWalker = new Vector2Int(walkerPos.x, walkerPos.y);
            UpdateWalkerPosition(ref copyWalker, randDirDir);
            repeated++;
            notValid = !ValidPos(copyWalker, minX, maxX, minY, maxY);
        } while (notValid && repeatR < 5);

        if (!notValid)
        {
            UpdateWalkerPosition(ref walkerPos, randDirDir);
        }
        currDir = randDirDir;
    }

    private bool ValidPos(Vector2Int walkerPos, int minX, int maxX, int minY, int maxY )
    {
        if ((walkerPos.x < minX || walkerPos.x < 0)
            || (walkerPos.y < minY || walkerPos.y < 0)
            || (walkerPos.x > maxX || walkerPos.x > length - 1)
            || (walkerPos.y > maxY || walkerPos.y > width - 1))
        {
            return false;
        }
        else if (mapData[walkerPos.x, walkerPos.y])
            return false;
        return true;
    }

    private Direction DirectionOfIndex(int index)
    {
        if(index == 0) { return Direction.left; }
        else if(index == 1) { return Direction.right; }
        else if(index == 2) { return Direction.down; }
        else if(index == 3) { return Direction.up; }
        return Direction.stay;
    }

    private Vector4 minMax(int x, int y)
    {
        int minX = x - 3 < 0 ? 0 : x - 3;
        int minY = y - 3 < 0 ? 0 : y - 3;
        int maxX = x + 3 > length - 1 ? length - 1 : x + 3;
        int maxY = y + 3 > width - 1 ? width - 1 : y + 3;
        return new Vector4(minX, maxX, minY, maxY);
    }
}
