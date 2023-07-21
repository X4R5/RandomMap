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
    MapManager currentMapManager;

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
        currentMapManager = currentMap.GetComponent<MapManager>();
        currentMapManager.entrySide = entrySide;
        currentMapManager.width = width;
        currentMapManager.height = height;
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                GameObject tile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                tile.transform.parent = currentMap.transform;
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    SetBorderTile(tile ,x, y, height, width);
                    currentMapManager.borderGrids.Add(new Vector2(x, y), tile.GetComponent<Tile>());
                    tile.GetComponent<BoxCollider2D>().isTrigger = false;
                    tile.layer = 6;
                }
                else
                {
                    currentMapManager.insideGrids.Add(new Vector2(x, y), tile.GetComponent<Tile>());
                    tile.layer = 7;

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
        return currentMapManager;
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
        tile.GetComponent<Tile>().usable = false;
        tile.GetComponent<BoxCollider2D>().enabled = true;
        tile.GetComponent<BoxCollider2D>().isTrigger = false;
        tile.layer = 6;
    }

    private void SetEntryTile(Vector2Int entrySide, bool setExitTile)
    {
        int y = 0, x = 0;
        Tile tile = null;
        List<Tile> tiles = new List<Tile>();
        Tile middleEndBridgeTile_ = null;

        if (entrySide == Vector2Int.left)
        {
            y = Random.Range(3, currentMapManager.height - 3);
            x = 0;
            tile = currentMapManager.borderGrids[new Vector2(0, y)];
            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            currentMapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var rightTopBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightTopBridgeTile.GetComponent<SpriteRenderer>().sprite = rightTopBridgeSprite;
            rightTopBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightTopBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].GetComponent<Tile>());
            //currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].GetComponent<Tile>().usable = false;
            //rightTopBridgeTile.GetComponent<Tile>().usable = false;

            var rightDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = rightDownBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y)].GetComponent<Tile>());


            currentMapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;
        }
        else if (entrySide == Vector2Int.right)
        {
            y = Random.Range(3, currentMapManager.height - 3);
            x = currentMapManager.width - 1;
            tile = currentMapManager.borderGrids[new Vector2(currentMapManager.width - 1, y)];

            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            currentMapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var leftTopBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftTopBridgeTile.GetComponent<SpriteRenderer>().sprite = leftTopBridgeSprite;
            leftTopBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftTopBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].GetComponent<Tile>());

            var leftDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = leftDownBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y)].GetComponent<Tile>());


            currentMapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;
        }
        else if (entrySide == Vector2Int.up)
        {
            x = Random.Range(3, currentMapManager.width - 3);
            y = currentMapManager.height - 1;
            tile = currentMapManager.borderGrids[new Vector2(x, y)];
            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;

            currentMapManager.borderGrids[new Vector2(x+1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            currentMapManager.borderGrids[new Vector2(x-1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].GetComponent<Tile>());

            var rightDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x, y - 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = topMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x, y - 1)].GetComponent<Tile>());
        }
        else if (entrySide == Vector2Int.down)
        {
            x = Random.Range(3, currentMapManager.width - 3);
            y = 0;
            tile = currentMapManager.borderGrids[new Vector2(x, y)];

            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;

            currentMapManager.borderGrids[new Vector2(x + 1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            currentMapManager.borderGrids[new Vector2(x - 1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].GetComponent<Tile>());

            var rightDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x, y + 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = downMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x, y + 1)].GetComponent<Tile>());
        }
        
        middleEndBridgeTile_.gameObject.name = "BridgeTile";
        middleEndBridgeTile_.GetComponent<SpriteRenderer>().sortingOrder = 1;

        foreach (var tile_ in tiles)
        {
            tile_.gameObject.name = "BridgeTile";
            tile_.gameObject.layer = 6;
            tile_.GetComponent<BoxCollider2D>().isTrigger = false;
            tile_.usable = false;
            if (tile_.GetComponent<SpriteRenderer>().sprite == baseSprite || tile_.GetComponent<SpriteRenderer>().sprite == secondSprite) continue;
            tile_.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        tile.gameObject.name = "EntryTile";
        tile.gameObject.layer = 6;
        tile.GetComponent<BoxCollider2D>().isTrigger = true;
        tile.GetComponent<Tile>().usable = true;
        currentMapManager.entryTilePos = new Vector2(x,y);
        if(setExitTile) SetExitTile(entrySide);
    }

    private void SetExitTile(Vector2Int entrySide)
    {
        List<Vector2Int> possibleExitSides = new List<Vector2Int>() { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        possibleExitSides.Remove(entrySide);
        Vector2Int exitSide = possibleExitSides[Random.Range(0, possibleExitSides.Count)];
        currentMapManager.exitSide = exitSide;

        int y = 0, x = 0;
        Tile tile = null;
        Tile middleEndBridgeTile_ = null;

        List<Tile> tiles = new List<Tile>();

        if (exitSide == Vector2Int.left)
        {
            y = Random.Range(3, currentMapManager.height - 3);
            x = 0;
            tile = currentMapManager.borderGrids[new Vector2(0, y)];

            if (!CheckAvailabilityForExitTile(Vector2Int.left)) { 
                SetExitTile(entrySide);
                return;
            }

            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;

            currentMapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var rightTopBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightTopBridgeTile.GetComponent<SpriteRenderer>().sprite = rightTopBridgeSprite;
            rightTopBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightTopBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].GetComponent<Tile>());

            var rightDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = rightDownBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y)].GetComponent<Tile>());


            currentMapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;

        }
        else if (exitSide == Vector2Int.right)
        {
            y = Random.Range(3, currentMapManager.height - 3);
            x = currentMapManager.width - 1;
            tile = currentMapManager.borderGrids[new Vector2(currentMapManager.width - 1, y)];

            tile.GetComponent<SpriteRenderer>().sprite = middleBridgeSprite;


            currentMapManager.borderGrids[new Vector2(x, y + 1)].GetComponent<SpriteRenderer>().sprite = topBridgeSprite;

            var leftTopBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftTopBridgeTile.GetComponent<SpriteRenderer>().sprite = leftTopBridgeSprite;
            leftTopBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftTopBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].GetComponent<Tile>());

            var leftDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = leftDownBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = middleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y)].GetComponent<Tile>());


            currentMapManager.borderGrids[new Vector2(x, y - 1)].GetComponent<SpriteRenderer>().sprite = downBridgeSprite;

        }
        else if (exitSide == Vector2Int.up)
        {
            x = Random.Range(3, currentMapManager.width - 3);
            y = currentMapManager.height - 1;
            tile = currentMapManager.borderGrids[new Vector2(x, currentMapManager.height - 1)];

            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;


            currentMapManager.borderGrids[new Vector2(x + 1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            currentMapManager.borderGrids[new Vector2(x - 1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y - 1)].GetComponent<Tile>());

            var rightDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = downRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y - 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x, y - 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = topMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x, y - 1)].GetComponent<Tile>());

            tile.gameObject.name = "ExitTile";
        }
        else if (exitSide == Vector2Int.down)
        {
            x = Random.Range(3, currentMapManager.width - 3);
            y = 0;
            tile = currentMapManager.borderGrids[new Vector2(x, 0)];

            tile.GetComponent<SpriteRenderer>().sprite = topDownMiddleBridgeSprite;

            currentMapManager.borderGrids[new Vector2(x + 1, y)].GetComponent<SpriteRenderer>().sprite = topdownRightBridgeSprite;
            currentMapManager.borderGrids[new Vector2(x - 1, y)].GetComponent<SpriteRenderer>().sprite = topdownLeftBridgeSprite;

            var leftDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].transform.position, Quaternion.identity);
            leftDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topLeftBridgeSprite;
            leftDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(leftDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x - 1, y + 1)].GetComponent<Tile>());

            var rightDownBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].transform.position, Quaternion.identity);
            rightDownBridgeTile.GetComponent<SpriteRenderer>().sprite = topRightBridgeSprite;
            rightDownBridgeTile.transform.parent = currentMap.transform;
            tiles.Add(rightDownBridgeTile.GetComponent<Tile>());
            tiles.Add(currentMapManager.insideGrids[new Vector2(x + 1, y + 1)].GetComponent<Tile>());

            var middleEndBridgeTile = Instantiate(tilePrefab, currentMapManager.insideGrids[new Vector2(x, y + 1)].transform.position, Quaternion.identity);
            middleEndBridgeTile.GetComponent<SpriteRenderer>().sprite = downMiddleEndBridgeSprite;
            middleEndBridgeTile.transform.parent = currentMap.transform;
            middleEndBridgeTile_ = middleEndBridgeTile.GetComponent<Tile>();
            //tiles.Add(middleEndBridgeTile.GetComponent<Tile>());
            //tiles.Add(currentMapManager.insideGrids[new Vector2(x, y + 1)].GetComponent<Tile>());
        }

        middleEndBridgeTile_.gameObject.name = "BridgeTile";
        middleEndBridgeTile_.GetComponent<SpriteRenderer>().sortingOrder = 1;

        foreach (var tile_ in tiles)
        {
            tile_.usable = false;
            tile_.gameObject.name = "BridgeTile";
            tile_.gameObject.layer = 6;
            tile_.GetComponent<BoxCollider2D>().isTrigger = false;
            if (tile_.GetComponent<SpriteRenderer>().sprite == baseSprite || tile_.GetComponent<SpriteRenderer>().sprite == secondSprite) continue;
            tile_.GetComponent<SpriteRenderer>().sortingOrder = 1;
        }

        tile.gameObject.name = "ExitTile";
        tile.GetComponent<Tile>().usable = true;
        tile.GetComponent<Tile>().gameObject.layer = 7;
        tile.GetComponent<BoxCollider2D>().isTrigger = true;
        currentMapManager.exitTilePos = new Vector2(x, y);
        //check fonksiyonuna iflerin icinde x y gondererek bakilacak
    }

    private bool CheckAvailabilityForExitTile(Vector2Int side)
    {
        if(side == Vector2Int.left)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentMapManager.borderGrids[new Vector2(0, currentMapManager.exitTilePos.y)].transform.position + new Vector3(-0.5f, 0), Vector2.left, currentMapManager.width);
            if (hit.collider != null)
            {
                Debug.Log("Hit " + hit.collider.name);
                return false;
            }
        }
        else if(side == Vector2Int.right)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentMapManager.borderGrids[new Vector2(currentMapManager.exitTilePos.x, currentMapManager.exitTilePos.y)].transform.position + new Vector3(0.5f, 0), Vector2.right, currentMapManager.width);
            if (hit.collider != null)
            {
                Debug.Log("Hit " + hit.collider.name);
                return false;
            }
        }
        else if(side == Vector2Int.up)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentMapManager.borderGrids[new Vector2(currentMapManager.exitTilePos.x, currentMapManager.exitTilePos.y)].transform.position + new Vector3(0, 0.5f), Vector2.up, currentMapManager.height);
            if (hit.collider != null)
            {
                Debug.Log("Hit " + hit.collider.name);
                return false;
            }
        }
        else if (side == Vector2Int.down)
        {
            RaycastHit2D hit = Physics2D.Raycast(currentMapManager.borderGrids[new Vector2(currentMapManager.exitTilePos.x, 0)].transform.position + new Vector3(0, -0.5f), Vector2.down, currentMapManager.height);
            if (hit.collider != null)
            {
                Debug.Log("Hit " + hit.collider.name);
                return false;
            }
        }
        return true;
    }
}
