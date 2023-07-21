using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] float _followSpeed = 0.1f, _mouseControlSpeed = 10f;
    Vector3 _targetPos, _dragOrigin;
    bool _followPlayer = true, _mouseControl = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _followPlayer = false;
        }

        if (Input.GetMouseButton(1))
        {
            _followPlayer = false;
            _mouseControl = true;
            _dragOrigin = Input.mousePosition;
        }
        else
        {
            _mouseControl = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _followPlayer = true;
        }
    }

    private void FixedUpdate()
    {
        if(_followPlayer)
        {
            _targetPos = new Vector3(_player.position.x, _player.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, _targetPos, _followSpeed * Time.deltaTime);
        }
        if (_mouseControl)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - _dragOrigin);
            Vector3 move = new Vector3(-pos.x * _mouseControlSpeed * Time.deltaTime, -pos.y * _mouseControlSpeed * Time.deltaTime, 0);

            transform.Translate(move, Space.World);
        }

    }
}
