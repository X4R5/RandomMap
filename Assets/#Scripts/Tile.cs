using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool _usable = true;
    public int _x, _y;
    [SerializeField] GameObject _mouseOverHighlight, _walkableHiglight;

    private void OnMouseOver()
    {
        if (!_usable) return;
        _mouseOverHighlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (!_usable) return;
        _mouseOverHighlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!_usable) return;
        if (!TurnManager.instance._isFree)
        {
            if (PlayerController.instance._isWalking) return;
            //if (!TurnManager.instance._isPlayerTurn) return;
        }
        if (TurnManager.instance._isFree)
        {
            PlayerController.instance.SetTargetMoveTile(this);
        }
        else if ((GameManager.instance._isShowingWalkableTiles && PlayerController.instance.GetCurrentTile().Neighbours().Contains(this)))
        {
            PlayerMove.instance.MoveToTargetTile(this);
        }

        GameManager.instance.HideWalkableTiles();
    }

    public void ShowWalkableHighlight()
    {
        if (!_usable) return;
        _walkableHiglight.SetActive(true);
    }

    public void HideWalkableHighlight()
    {
        if (!_usable) return;
        _walkableHiglight.SetActive(false);
    }

    public List<Tile> Neighbours()
    {
        var mapManager = transform.parent.GetComponent<MapManager>();
        if (mapManager == null) return null;

        var neighbours = new List<Tile>();

        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                if (x == 0 && y == 0) continue;

                Tile neighbour = null;
                mapManager.insideGrids.TryGetValue(new Vector2(_x + x, _y + y), out neighbour);
                if (neighbour != null)
                {
                    
                    neighbours.Add(neighbour);
                }
            }
        }

        return neighbours;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_usable) return;
        if (collision.CompareTag("Player"))
        {
            PlayerController.instance.SetCurrentTile(this);
        }
    }
}
