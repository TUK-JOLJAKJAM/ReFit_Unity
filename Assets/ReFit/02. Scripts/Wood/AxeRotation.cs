using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxeRotation : MonoBehaviour
{
    // ========== Inspector UI ===============


    // ========== Hidden Datas ===============
    // 도끼 회전
    private float _minRotationZ = 8f;
    private float _maxRotationZ = -60f;
    private float _rotationSpeed = 20f;

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
        Debug.Log("코루틴 시작");
        // 1. 코루틴 시작 시점에 마우스 시작 위치를 현재 위치로 강제 초기화
        _mouseStartPoint = Mouse.current.position.ReadValue();

        // 2. 현재 각도 가져오기 (보정 포함)
        float currentZ = transform.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        yield return null;

        while (Mouse.current.leftButton.isPressed && _isDragging)
        {
            _mouseEndPoint = Mouse.current.position.ReadValue();

            // 마우스 이동량 계산
            float distanceValue = _mouseEndPoint.x - _mouseStartPoint.x;

            // 마우스 이동이 거의 없다면 계산 건너뛰기 (미세 떨림 방지)
            if (true)
            {
                // 회전량 누적
                currentZ -= distanceValue * Time.deltaTime * _rotationSpeed;

                // 3. Clamp 인자 순서 주의! (작은 값, 큰 값 순서여야 함)
                float min = Mathf.Min(_minRotationZ, _maxRotationZ);
                float max = Mathf.Max(_minRotationZ, _maxRotationZ);
                currentZ = Mathf.Clamp(currentZ, min, max);

                // 적용
                transform.localRotation = Quaternion.Euler(0, 0, currentZ);
            }

            // 4. 다음 프레임을 위해 시작점 갱신
            _mouseStartPoint = _mouseEndPoint;

            yield return null;
        }
    }


}
