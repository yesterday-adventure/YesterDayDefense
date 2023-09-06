using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    [SerializeField] float _moveSpeed = 2f;
    Vector3 _dir;
    Vector3 _lastMousePos;
    Vector3 _mousePos;
    private void LateUpdate()
    {
        if(Input.GetMouseButtonDown(1))
        {
            _lastMousePos = Input.mousePosition;
            _mousePos = Input.mousePosition;
        }

        if(Input.GetMouseButton(1))
        {
            _lastMousePos = Input.mousePosition;
            _dir = _mousePos - _lastMousePos;
            _dir.z = _dir.y;
            _dir.y = 0;
            transform.position += _dir * _moveSpeed * Time.deltaTime;
            _mousePos = _lastMousePos;
        }
    }
}