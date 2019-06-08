using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnmyTileColData : MonoBehaviour
{
    public int maxLength = 25;
    public int maxWidth = 25;
    public Vector2Int minOrigin = Vector2Int.zero;
    public Vector2Int maxOrigin = Vector2Int.zero;

    private Tilemap enemyColMap;

    private void Awake()
    {
        enemyColMap = GetComponent<Tilemap>();
    }
}
