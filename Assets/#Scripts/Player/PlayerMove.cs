using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove instance;
    bool _isMoving = false;

    private void Awake()
    {
        instance = this;
    }

    public async void MoveToTargetTile(Tile targetTile)
    {
        if (targetTile == null) return;
        if (TurnManager.instance._isFree) return;
        while (PlayerController.instance.GetCurrentTile() != targetTile && !_isMoving)
        {
            _isMoving = true;
            await MoveToNextTile(PlayerController.instance.GetCurrentTile(), targetTile);
        }
    }

    private async Task MoveToNextTile(Tile currentTile, Tile targetTile)
    {
        if (currentTile == null) return;
        if (targetTile == null) return;
        await Task.Yield();
        var mapManager = currentTile.GetComponentInParent<MapManager>();


        var nextTile = targetTile._x > currentTile._x ? new Vector2(currentTile._x + 1, currentTile._y) :
            targetTile._x < currentTile._x ? new Vector2(currentTile._x - 1, currentTile._y) :
            targetTile._y > currentTile._y ? new Vector2(currentTile._x, currentTile._y + 1) :
            targetTile._y < currentTile._y ? new Vector2(currentTile._x, currentTile._y - 1) :
            new Vector2(currentTile._x, currentTile._y);

        PlayerController.instance.transform.position = mapManager.insideGrids[nextTile].transform.position;
        await Task.Delay(700);
        await Task.Yield();
        PlayerController.instance.SetCurrentTile(mapManager.insideGrids[nextTile].GetComponent<Tile>());
        _isMoving = false;
        //if (targetTile._x > currentTile._x)
        //{
        //    PlayerController.instance.transform.position = mapManager.insideGrids[new Vector2(currentTile._x + 1, currentTile._y)].transform.position;
        //}
        //else if(targetTile._x < currentTile._x)
        //{
        //    PlayerController.instance.transform.position = mapManager.insideGrids[new Vector2(currentTile._x - 1, currentTile._y)].transform.position;
        //}
        //else if(targetTile._y > currentTile._y)
        //{
        //    PlayerController.instance.transform.position = mapManager.insideGrids[new Vector2(currentTile._x, currentTile._y + 1)].transform.position;
        //}
        //else if (targetTile._y < currentTile._y)
        //{
        //    PlayerController.instance.transform.position = mapManager.insideGrids[new Vector2(currentTile._x, currentTile._y - 1)].transform.position;
        //}
    }
}
