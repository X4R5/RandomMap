using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] int mapCount;
    [SerializeField] int minWidth, maxWidth, minHeight, maxHeight;
    [SerializeField] GameObject leftBridgePrefab, rightBridgePrefab, topBridgePrefab, downBridgePrefab;
    MapManager lastMapManager = null;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            lastMapManager = null;
            DestroyCurrentMaps();
            CreateMaps();
        }
    }

    private void DestroyCurrentMaps()
    {
        var currentMaps = GameObject.FindGameObjectsWithTag("Map");
        foreach (var map in currentMaps)
        {
            Destroy(map);
        }
    }

    private void CreateMaps()
    {

        for (int i = 0; i < mapCount; i++)
        {
            bool setExitTile = i == mapCount - 1 ? false : true;
            int width = Random.Range(minWidth, maxWidth);
            int height = Random.Range(minHeight, maxHeight);

            var entrySide = SelectEntrySide();
            var newMap = GridManager.Instance.CreateMap(entrySide, width, height, setExitTile);

            ConnectMapToLastMap(newMap);

            lastMapManager = newMap;
        }
    }

    private void ConnectMapToLastMap(MapManager newMap)
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
