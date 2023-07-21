using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Pathfinding;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [SerializeField] int mapCount;
    [SerializeField] int minWidth, maxWidth, minHeight, maxHeight;
    [SerializeField] GameObject leftBridgePrefab, rightBridgePrefab, topBridgePrefab, downBridgePrefab, creatingMapsCanvas;
    MapManager lastMapManager = null;
    bool isAvailable = false;
    public bool _isShowingWalkableTiles = false;

    List<Tile> _walkableTiles = new List<Tile>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        CreateMaps();
    }


    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    lastMapManager = null;
        //    DestroyCurrentMaps();
        //    CreateMaps();
        //}
    }

    public void ShowWalkableTiles()
    {
        if (TurnManager.instance._isFree) return;
        if (_isShowingWalkableTiles)
        {
            HideWalkableTiles();
            return;
        }
        if (PlayerController.instance.GetCurrentTile() == null) return;
        if (PlayerController.instance.GetCurrentTile().transform.parent.GetComponent<MapManager>() == null) return;
        if (PlayerController.instance._isWalking) return;

        SelectWalkableTiles();

        foreach (var tile in _walkableTiles)
        {
            tile.ShowWalkableHighlight();
        }
        PlayerController.instance._canWalk = true;
        _isShowingWalkableTiles = true;
        //PlayerController.instance._canWalk = true;
    }
    public void HideWalkableTiles()
    {
        foreach (var tile in _walkableTiles)
        {
            tile.HideWalkableHighlight();
        }
        _isShowingWalkableTiles = false;
    }

    void SelectWalkableTiles()
    {
        _walkableTiles.Clear();
        foreach (Tile tile in PlayerController.instance.GetCurrentTile().Neighbours())
        {
            _walkableTiles.Add(tile);
        }
    }

    async Task ScanAstar()
    {
        var astarPath = GameObject.FindGameObjectWithTag("Astar").GetComponent<AstarPath>();
        astarPath.Scan();
        await Task.Yield();
    }

    async Task ChangeAstarGridSize()
    {
        var astarPath = GameObject.FindGameObjectWithTag("Astar")?.GetComponent<AstarPath>();
        var newWidth = FintRightTileIndex() - FintLeftTileIndex();
        var newDepth = FintTopTileIndex() - FintBottomTileIndex();
        if (astarPath != null)
        {
            astarPath.data.gridGraph.SetDimensions(((int)newWidth * 2) + 10, ((int)newDepth * 2) + 10, astarPath.data.gridGraph.nodeSize);
            var center = new Vector3(FintLeftTileIndex() + (newWidth / 2), FintBottomTileIndex() + (newDepth / 2), astarPath.data.gridGraph.center.z);
            astarPath.data.gridGraph.center = center;
        }

        await Task.Yield();
    }

    float FintTopTileIndex()
    {
        float topTileY = -9999;
        var allTileObjects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in allTileObjects)
        {
            if (tile.transform.position.y > topTileY)
            {
                topTileY = tile.transform.position.y;
            }
        }
        return topTileY + 0.5f;
    }
    float FintBottomTileIndex()
    {
        float Y = 9999;
        var allTileObjects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in allTileObjects)
        {
            if (tile.transform.position.y < Y)
            {
                Y = tile.transform.position.y;
            }
        }
        return Y + 0.5f;
    }

    float FintRightTileIndex()
    {
        float X = -9999;
        var allTileObjects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in allTileObjects)
        {
            if (tile.transform.position.x > X)
            {
                X = tile.transform.position.x;
            }
        }
        return X + 0.5f;
    }

    float FintLeftTileIndex()
    {
        float X = 9999;
        var allTileObjects = GameObject.FindGameObjectsWithTag("Tile");
        foreach (var tile in allTileObjects)
        {
            if (tile.transform.position.x < X)
            {
                X = tile.transform.position.x;
            }
        }
        return X + 0.5f;
    }

    private void DestroyCurrentMaps()
    {
        lastMapManager = null;
        var currentMaps = GameObject.FindGameObjectsWithTag("Map");
        foreach (var map in currentMaps)
        {
            Destroy(map);
        }
    }

    private async void CreateMaps()
    {
        creatingMapsCanvas.SetActive(true);
        int k = 0;
        for (int i = 0; i < mapCount; i++)
        {
            bool setExitTile = i == mapCount - 1 ? false : true;
            int width = Random.Range(minWidth, maxWidth);
            int height = Random.Range(minHeight, maxHeight);

            var entrySide = SelectEntrySide();
            var newMap = GridManager.Instance.CreateMap(entrySide, width, height, setExitTile);
            newMap.name = "Map" + k;
            k++;

            //if(k >= 50)
            //{
            //    DestroyCurrentMaps();
            //    CreateMaps();
            //}

            await ConnectMapToLastMap(newMap);

            await CheckIfAvailable(newMap);

            if (!isAvailable)
            {
                Destroy(newMap.gameObject);
                i--;
                continue;
                //Debug.Log(newMap.name);
            }

            //if(newMap.exitSide == Vector2.left)
            //{
            //    width *= -1;
            //}
            //else if(newMap.exitSide == Vector2.down)
            //{
            //    height *= -1;
            //}

            lastMapManager = newMap;
        }
        await ChangeAstarGridSize();
        await ScanAstar();
        await Task.Delay(1000);
        creatingMapsCanvas.SetActive(false);
    }



    private async Task CheckIfAvailable(MapManager newMap)
    {
        isAvailable = true;

        await CheckBorderTiles(newMap);
        await CheckInsideTiles(newMap);


        await Task.Yield();
    }

    private async Task CheckBorderTiles(MapManager newMap)
    {
        foreach (var tile in newMap.borderGrids.Keys)
        {
            await CheckDownSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
            await CheckUpSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
            await CheckLeftSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
            await CheckRightSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
        }
        await Task.Yield();
    }

    private async Task CheckInsideTiles(MapManager newMap)
    {
        foreach (var tile in newMap.insideGrids.Keys)
        {
            await CheckInsideDownSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
            await CheckInsideUpSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
            await CheckInsideLeftSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
            await CheckInsideRightSide(newMap, tile);
            if (!isAvailable)
            {
                break;
            }
        }
        await Task.Yield();
    }

    private async Task CheckInsideRightSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("right");
        RaycastHit2D hit = Physics2D.Raycast(newMap.insideGrids[tile].transform.position + new Vector3(0.5f, 0), Vector2.right, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.insideGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("right + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task CheckInsideLeftSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("right");
        RaycastHit2D hit = Physics2D.Raycast(newMap.insideGrids[tile].transform.position + new Vector3(0.5f, 0), Vector2.left, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.insideGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("right + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task CheckInsideUpSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("right");
        RaycastHit2D hit = Physics2D.Raycast(newMap.insideGrids[tile].transform.position + new Vector3(0.5f, 0), Vector2.up, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.insideGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("right + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task CheckInsideDownSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("right");
        RaycastHit2D hit = Physics2D.Raycast(newMap.insideGrids[tile].transform.position + new Vector3(0.5f, 0), Vector2.down, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.insideGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("right + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task CheckRightSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("right");
        RaycastHit2D hit = Physics2D.Raycast(newMap.borderGrids[tile].transform.position + new Vector3(0.5f, 0), Vector2.right, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.borderGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("right + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task CheckLeftSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("left");
        RaycastHit2D hit = Physics2D.Raycast(newMap.borderGrids[tile].transform.position + new Vector3(-0.5f, 0), Vector2.left, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.borderGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("left + " + tile + " " + newMap.name);
            }
        }

        await Task.Yield();
    }

    private async Task CheckUpSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("up");
        RaycastHit2D hit = Physics2D.Raycast(newMap.borderGrids[tile].transform.position + new Vector3(0, 0.5f), Vector2.up, 1f);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.borderGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("Left + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task CheckDownSide(MapManager newMap, Vector2 tile)
    {
        Debug.Log("down");
        RaycastHit2D hit = Physics2D.Raycast(newMap.borderGrids[tile].transform.position + new Vector3(0, -0.5f), Vector2.down, 1);
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("Tile"))
            {
                if (newMap.borderGrids[tile].transform.parent == hit.collider.transform.parent || hit.collider.transform.parent.CompareTag("Bridge"))
                {
                    return;
                }
                isAvailable = false;
                Debug.Log("Down + " + tile + " " + newMap.name);
            }
        }
        await Task.Yield();
    }

    private async Task ConnectMapToLastMap(MapManager newMap)
    {
        Vector3 offset = Vector3.zero;
        if(lastMapManager == null)
        {
            var startBridge = GameObject.Find("StartBridge");
            offset = startBridge.transform.position - newMap.borderGrids[newMap.entryTilePos].transform.position;
            newMap.transform.position += offset;
        }
        else
        {
            var newBridge = CreateBridge();
            newBridge.transform.position = lastMapManager.borderGrids[lastMapManager.exitTilePos].transform.position;
            newBridge.transform.parent = lastMapManager.transform;
            offset = newBridge.transform.Find("EndPos").position - newMap.borderGrids[newMap.entryTilePos].transform.position;
            newMap.transform.position += offset;
        }
        await Task.Yield();
    }

    GameObject CreateBridge()
    {
        GameObject bridge = null;
        if (lastMapManager.exitSide == Vector2Int.left)
        {
            bridge = Instantiate(leftBridgePrefab);
        }
        else if(lastMapManager.exitSide == Vector2Int.right)
        {
            bridge = Instantiate(rightBridgePrefab);
        }
        else if(lastMapManager.exitSide == Vector2Int.up)
        {
            bridge = Instantiate(topBridgePrefab);
        }
        else if (lastMapManager.exitSide == Vector2Int.down)
        {
            bridge = Instantiate(downBridgePrefab);
        }

        return bridge;
    }

    private Vector2Int SelectEntrySide()
    {
        Vector2Int entrySide = Vector2Int.zero;
        if (lastMapManager == null)
        {
            entrySide = Vector2Int.left;
        }
        else
        {
            if (lastMapManager.exitSide == Vector2Int.up)
            {
                entrySide = Vector2Int.down;
            }
            else if (lastMapManager.exitSide == Vector2Int.down)
            {
                entrySide = Vector2Int.up;
            }
            else if (lastMapManager.exitSide == Vector2Int.left)
            {
                entrySide = Vector2Int.right;
            }
            else if (lastMapManager.exitSide == Vector2Int.right)
            {
                entrySide = Vector2Int.left;
            }
        }
        return entrySide;
    }
}
