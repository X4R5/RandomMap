using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [SerializeField] GameObject tilePrefab, mapPrefab;
    [SerializeField] Sprite baseSprite, secondSprite;

    [Header("Wall Sprites")]
    [SerializeField] Sprite leftWallSprite;
    [SerializeField] Sprite topWallSprite, rightWallSprite, downWallSprite, leftTopWallSprite, rightTopWallSprite, leftDownWallSprite, rigtDownWallSprite;

    [Header("Bridge Sprites")]
    [SerializeField] Sprite leftTopBridgeSprite;
    [SerializeField] Sprite rightTopBridgeSprite, leftDownBridgeSprite, rightDownBridgeSprite, topBridgeSprite, downBridgeSprite, middleBridgeSprite, middleEndBridgeSprite;
    [SerializeField] Sprite topMiddleBridgeSprite, topLeftBridgeSprite, topRightBridgeSprite;
    [SerializeField] Sprite downMiddleBridgeSprite, downLeftBridgeSprite, downRightBridgeSprite;
    [SerializeField] Sprite topdownLeftBridgeSprite, topdownRightBridgeSprite, topDownMiddleBridgeSprite, topMiddleEndBridgeSprite, downMiddleEndBridgeSprite;

    GameObject currentMap;
    MapManager mapManager;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {

    }

    public MapManager CreateMap(Vector2Int entrySide ,int width, int height, bool addExitTile)
    {
        currentMap = Instantiate(mapPrefab, new Vector3(width / 2, height / 2, 0), Quaternion.identity);
        mapManager = currentMap.GetComponent<MapManager>();
        mapManager.entrySide = entrySide;
        mapManager.width = width;
        mapManager.height = height;
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                tile.transform.parent = currentMap.transform;
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    SetBorderTile(tile ,x, y, height, width);
                    mapManager.borderGrids.Add(new Vector2(x, y), tile.GetComponent<Tile>());
                    tile.GetComponent<BoxCollider2D>().enabled = true;
                }
                else
                {
                    mapManager.insideGrids.Add(new Vector2(x, y), tile.GetComponent<Tile>());

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        tile.GetComponent<SpriteRenderer>().sprite = baseSprite;
                    }
                    else if (x % 2 == 1 && y % 2 == 1)
                    {
                        tile.GetComponent<SpriteRenderer>().sprite = baseSprite;
                    }
                    else
                    {
                        tile.GetComponent<SpriteRenderer>().sprite = secondSprite;
                    }
                }
            }
        }
        SetEntryTile(entrySide, addExitTile);
        return mapManager;
    }

    private void SetBorderTile(GameObject tile, int x, int y, int height, int width)
    {
        if(y == 0)
        {
            if(x == 0)
            {
                tile.GetComponent<SpriteRenderer>().sprite = leftDownWallSprite;
            }else if(x == width - 1)
            {
                tile.GetComponent<SpriteRenderer>().sprite = rigtDownWallSprite;
            }
            else
            {
                tile.GetComponent<SpriteRenderer>().sprite = downWallSprite;
            }
        }else if(y ==  height - 1)
        {
              if (x == 0)
            {
                tile.GetComponent<SpriteRenderer>().sprite = leftTopWallSprite;
            }
            else if (x == width - 1)
            {
                tile.GetComponent<SpriteRenderer>().sprite = rightTopWallSprite;
            }
            else
            {
                tile.GetComponent<SpriteRenderer>().sprite = topWallSprite;
            }
        }
        else
        {
            if (x == 0)
            {
                tile.GetComponent<SpriteRenderer>().sprite = leftWallSprite;
            }
            else if (x == width - 1)
            {
                tile.GetComponent<SpriteRenderer>().sprite = rightWallSprite;
            }
        }
    }

    private void SetEntryTile(Vector2Int entrySide, bool setExitTile)
    {
        int y = 0, x = 0;
        Tile tile = new Tile();

        if (entrySide == Vector2Int.left)
        {
            y = Random.Range(3, mapManager.height - 3);
            x = 0;
            tile = mapManager.borderGrids[new Vector2(0, y)];
            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            mapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var rightTopBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightTopBridgeTile.GetComponent<SpriteRenderer>().sprite = rightTopBridgeSprite;
            rightTopBridgeTile.transform.parent = currentMap.transform;

            var rightDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = rightDownBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;

            
            mapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;
        }
        else if (entrySide == Vector2Int.right)
        {
            y = Random.Range(3, mapManager.height - 3);
            x = mapManager.width - 1;
            tile = mapManager.borderGrids[new Vector2(mapManager.width - 1, y)];

            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            mapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var leftTopBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftTopBridgeTile.GetComponent<SpriteRenderer>().sprite = leftTopBridgeSprite;
            leftTopBridgeTile.transform.parent = currentMap.transform;

            var leftDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = leftDownBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;


            mapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;
        }
        else if (entrySide == Vector2Int.up)
        {
            x = Random.Range(3, mapManager.width - 3);
            y = mapManager.height - 1;
            tile = mapManager.borderGrids[new Vector2(x, y)];
            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;
            mapManager.borderGrids[new Vector2(x+1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            mapManager.borderGrids[new Vector2(x-1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;

            var rightDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x, y - 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = topMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
        }
        else if (entrySide == Vector2Int.down)
        {
            x = Random.Range(3, mapManager.width - 3);
            y = 0;
            tile = mapManager.borderGrids[new Vector2(x, y)];

            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;
            mapManager.borderGrids[new Vector2(x + 1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            mapManager.borderGrids[new Vector2(x - 1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;

            var rightDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x, y + 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = downMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
        }
        
        tile.GetComponent<BoxCollider2D>().enabled = false;
        tile.gameObject.name = "EntryTile";
        mapManager.entryTilePos = new Vector2(x,y);
        if(setExitTile) SetExitTile(entrySide);
    }

    private void SetExitTile(Vector2Int entrySide)
    {
        List<Vector2Int> possibleExitSides = new List<Vector2Int>() { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        possibleExitSides.Remove(entrySide);
        Vector2Int exitSide = possibleExitSides[Random.Range(0, possibleExitSides.Count)];
        mapManager.exitSide = exitSide;
        int y = 0, x = 0;
        Tile tile = new Tile();
        if (exitSide == Vector2Int.left)
        {
            y = Random.Range(3, mapManager.height - 3);
            x = 0;
            tile = mapManager.borderGrids[new Vector2(0, y)];

            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            mapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var rightTopBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightTopBridgeTile.GetComponent<SpriteRenderer>().sprite = rightTopBridgeSprite;
            rightTopBridgeTile.transform.parent = currentMap.transform;

            var rightDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = rightDownBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;


            mapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;

        }
        else if (exitSide == Vector2Int.right)
        {
            y = Random.Range(3, mapManager.height - 3);
            x = mapManager.width - 1;
            tile = mapManager.borderGrids[new Vector2(mapManager.width - 1, y)];
            tile.gameObject.name = "ExitTile";
            
            tile = mapManager.borderGrids[new Vector2(mapManager.width - 1, y)];

            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            mapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var leftTopBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftTopBridgeTile.GetComponent<SpriteRenderer>().sprite = leftTopBridgeSprite;
            leftTopBridgeTile.transform.parent = currentMap.transform;

            var leftDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = leftDownBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;


            mapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;

        }
        else if (exitSide == Vector2Int.up)
        {
            x = Random.Range(3, mapManager.width - 3);
            y = mapManager.height - 1;
            tile = mapManager.borderGrids[new Vector2(x, mapManager.height - 1)];

            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;
            mapManager.borderGrids[new Vector2(x + 1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            mapManager.borderGrids[new Vector2(x - 1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;

            var rightDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x, y - 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = topMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            tile.gameObject.name = "ExitTile";
        }
        else if (exitSide == Vector2Int.down)
        {
            x = Random.Range(3, mapManager.width - 3);
            y = 0;
            tile = mapManager.borderGrids[new Vector2(x, 0)];

            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;
            mapManager.borderGrids[new Vector2(x + 1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            mapManager.borderGrids[new Vector2(x - 1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;

            var rightDownBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;

            var middleEndBridgeTile = Instantiate(tilePrefab, mapManager.insideGrids[new Vector2(x, y + 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = downMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
        }
        tile.gameObject.name = "ExitTile";
        mapManager.exitTilePos = new Vector2(x, y);
    }
}
