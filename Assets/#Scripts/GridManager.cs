using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab, mapPrefab;
    [SerializeField] int minWidth, maxWidth, minHeight, maxHeight;
    [SerializeField] Color baseColor, secondColor;
    GameObject currentMap;
    MapManager mapManager;
    private void Start()
    {
        int width = Random.Range(minWidth, maxWidth);
        int height = Random.Range(minHeight, maxHeight);
        CreateMap(width, height);
    }

    public void CreateMap(int width, int height)
    {
        currentMap = Instantiate(mapPrefab, new Vector3(width / 2, height / 2, 0), Quaternion.identity);
        mapManager = currentMap.GetComponent<MapManager>();
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
                    tile.GetComponent<SpriteRenderer>().color = Color.black;
                    mapManager.borderGrids.Add(new Vector2(x, y), tile.GetComponent<Tile>());
                    tile.GetComponent<BoxCollider2D>().enabled = true;
                }
                else
                {
                    mapManager.insideGrids.Add(new Vector2(x, y), tile.GetComponent<Tile>());

                    if (x % 2 == 0 && y % 2 == 0)
                    {
                        tile.GetComponent<SpriteRenderer>().color = baseColor;
                    }
                    else if (x % 2 == 1 && y % 2 == 1)
                    {
                        tile.GetComponent<SpriteRenderer>().color = baseColor;
                    }
                    else
                    {
                        tile.GetComponent<SpriteRenderer>().color = secondColor;
                    }
                }
            }
        }

        var randomEntrySide = new List<Vector2Int>() { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        SetEntryTile(randomEntrySide[Random.Range(0, randomEntrySide.Count)]);
    }

    private void SetEntryTile(Vector2Int entrySide)
    {
        if (entrySide == Vector2Int.left)
        {
            int y = Random.Range(1, mapManager.height - 2);
            var tile = mapManager.borderGrids[new Vector2(0, y)];
            tile.GetComponent<SpriteRenderer>().color = Color.red;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "EntryTile";
        }
        else if (entrySide == Vector2Int.right)
        {
            int y = Random.Range(1, mapManager.height - 2);
            var tile = mapManager.borderGrids[new Vector2(mapManager.width - 1, y)];
            tile.GetComponent<SpriteRenderer>().color = Color.red;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "EntryTile";
        }
        else if (entrySide == Vector2Int.up)
        {
            int x = Random.Range(1, mapManager.width - 2);
            var tile = mapManager.borderGrids[new Vector2(x, mapManager.height - 1)];
            tile.GetComponent<SpriteRenderer>().color = Color.red;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "EntryTile";
        }
        else if (entrySide == Vector2Int.down)
        {
            int x = Random.Range(1, mapManager.width - 2);
            var tile = mapManager.borderGrids[new Vector2(x, 0)];
            tile.GetComponent<SpriteRenderer>().color = Color.red;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "EntryTile";
        }

        SetExitTile(entrySide);
    }

    private void SetExitTile(Vector2Int entrySide)
    {
        List<Vector2Int> possibleExitSides = new List<Vector2Int>() { Vector2Int.left, Vector2Int.up, Vector2Int.right, Vector2Int.down };
        possibleExitSides.Remove(entrySide);

        Vector2Int exitSide = possibleExitSides[Random.Range(0, possibleExitSides.Count)];

        if (exitSide == Vector2Int.left)
        {
            int y = Random.Range(1, mapManager.height - 2);
            var tile = mapManager.borderGrids[new Vector2(0, y)];
            tile.GetComponent<SpriteRenderer>().color = Color.green;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "ExitTile";
        }
        else if (exitSide == Vector2Int.right)
        {
            int y = Random.Range(1, mapManager.height - 2);
            var tile = mapManager.borderGrids[new Vector2(mapManager.width - 1, y)];
            tile.GetComponent<SpriteRenderer>().color = Color.green;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "ExitTile";
        }
        else if (exitSide == Vector2Int.up)
        {
            int x = Random.Range(1, mapManager.width - 2);
            var tile = mapManager.borderGrids[new Vector2(x, mapManager.height - 1)];
            tile.GetComponent<SpriteRenderer>().color = Color.green;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "ExitTile";
        }
        else if (exitSide == Vector2Int.down)
        {
            int x = Random.Range(1, mapManager.width - 2);
            var tile = mapManager.borderGrids[new Vector2(x, 0)];
            tile.GetComponent<SpriteRenderer>().color = Color.green;
            tile.GetComponent<BoxCollider2D>().enabled = false;
            tile.gameObject.name = "ExitTile";
        }
    }
}
