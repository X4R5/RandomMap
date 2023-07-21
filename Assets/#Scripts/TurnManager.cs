using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    public bool _isFree = true, _isPlayerTurn = true;


    private void Awake()
    {
        instance = this;
    }
}
