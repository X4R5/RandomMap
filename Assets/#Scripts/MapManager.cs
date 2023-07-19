using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{

    public Dictionary<Vector2, Tile> insideGrids = new Dictionary<Vector2, Tile>();
    public Dictionary<Vector2, Tile> borderGrids = new Dictionary<Vector2, Tile>();
    public int width, height;
    public Vector2 entryTilePos, exitTilePos;
    public Vector2Int entrySide, exitSide;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
