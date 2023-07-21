using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public bool _canWalk = true, _isWalking = false;

    Tile _currentTile, _targetMoveTile;

    [SerializeField] Tile _startTile;
    [SerializeField] float _moveSpeed = 1f;
    Path _path;
    Seeker _seeker;
    Rigidbody2D _rb;
    private void Awake()
    {
        instance = this;
        _currentTile = _startTile;
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {

        if (!_isWalking) return;
        _seeker.StartPath(_rb.position, _targetMoveTile.transform.position, OnPathComplete);
        if (_path == null) return;
        if (_path.vectorPath.Count == 0) return;
        Vector2 direction = ((Vector2)_path.vectorPath[0] - _rb.position).normalized;
        Vector2 force = direction * _moveSpeed * Time.deltaTime;
        _rb.AddForce(force);
        float distance = Vector2.Distance(_rb.position, _path.vectorPath[0]);
        if (distance < 1f)
        {
            _path.vectorPath.RemoveAt(0);
        }
    }

    public Tile GetCurrentTile()
    {
        return _currentTile;
    }

    private void OnPathComplete(Path p)
    {
        _isWalking = false;
    }

    public void SetCurrentTile(Tile tile)
    {
        _currentTile = tile;
    }

    public void SetTargetMoveTile(Tile tile)
    {
        if (!_canWalk) return;
        if(_isWalking) return;

        _targetMoveTile = tile;
        _isWalking = true;
    }
}
