using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool usable = true;
    [SerializeField] GameObject highlight;

    private void OnMouseOver()
    {
        if (!usable) return;
        highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (!usable) return;
        highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!usable) return;
        PlayerController.instance.SetTargetMoveTile(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!usable) return;
        if (other.CompareTag("Player"))
        {
            PlayerController.instance.SetCurrentTile(this);
        }
    }
}
