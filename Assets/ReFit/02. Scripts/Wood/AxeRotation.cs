using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeRotation : MonoBehaviour
{
    // ========== Inspector UI ===============


    // ========== Hidden Datas ===============
    // 도끼 회전
    private float _minRotationZ = 12f;
    private float _maxRotationZ = -69f;
    private float _rotationSpeed = 10f;

    // 마우스 드래그
    private Vector2 _mouseStartPoint = Vector2.zero;
    private Vector2 _mouseEndPoint = Vector2.zero;
    bool _isDragging = false;


    // ========== Functions ===============
    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) 
        {
            Debug.Log("Mouse Pressed");
            _mouseStartPoint = Mouse.current.position.ReadValue();
            StartCoroutine(DragAxeRotate());
            _isDragging = true;
        }

        if(Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
        {
            Debug.Log("마우스 뗌");
            _isDragging = false;
        }
    }

    IEnumerator DragAxeRotate()
    {
        Debug.Log($"{_mouseStartPoint}, {_mouseEndPoint}");
        yield return null;

        while (Mouse.current.leftButton.isPressed && _isDragging)
        {
            Debug.Log("드래그 반복문 진입");
            _mouseEndPoint = Mouse.current.position.ReadValue();

            float distanceValue = _mouseEndPoint.x - _mouseStartPoint.x;
            if(transform.rotation.z <= _minRotationZ && transform.rotation.z >= _maxRotationZ)
            transform.Rotate(0, 0, -distanceValue * Time.deltaTime * _rotationSpeed);

            _mouseStartPoint = _mouseEndPoint;

            yield return null;
        }
    }


}
