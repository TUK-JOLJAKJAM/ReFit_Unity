using UnityEngine;

[System.Serializable]
public class GyroHud : MonoBehaviour, IReFitUI, IReFitGyro
{
    GameManager gameManager;
    public RectTransform hudPointer;
    public GameObject hudUp;
    public GameObject hudDown;
    public GameObject hudLeft;
    public GameObject hudRight;

    float pointerOffsetMax = 40.0f;

    public enum GyroDirection
    {
        Up,
        Down,
        Left,
        Right,
        Center
    }

    //----------IReFitUI--------------
    public void Initialize()
    {
        gameManager = GameManager.instance;
    }
    public void UpdateUI()
    {
        //테스트모드에서는 클릭하여 포인터 이동하도록 구현, 실제 게임에서는 자이로 센서 값에 따라 포인터 이동
        if (gameManager.TestHandler.isTestMode)
        {
            if (Input.GetMouseButton(0))
            {
                //클릭중, 마우스의 좌표 출력
                ReFItLogger.Info($"Mouse Position: {Input.mousePosition}");

                // 마우스의 스크린 좌표를 포인터 부모(Canvas) 기준의 로컬 좌표로 변환
                RectTransform parentRect = hudPointer.parent as RectTransform;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, Input.mousePosition, null, out Vector2 localPoint))
                {
                    // 포인터가 최대 반경(pointerOffsetMax)을 벗어나지 않도록 제한
                    localPoint = Vector2.ClampMagnitude(localPoint, pointerOffsetMax);

                    // 자이로와 비슷하게 부드럽게 마우스 위치로 이동
                    hudPointer.anchoredPosition = Vector2.Lerp(hudPointer.anchoredPosition, localPoint, Time.deltaTime * 10.0f);
                }
            }
            else
            {
                // 마우스를 떼면 다시 중앙으로 부드럽게 복귀
                hudPointer.anchoredPosition = Vector2.Lerp(hudPointer.anchoredPosition, Vector2.zero, Time.deltaTime * 5.0f);
            }
        }
        else
        {
            hudPointer.anchoredPosition = Vector2.Lerp(hudPointer.anchoredPosition,
                new Vector2(gameManager.GyroManager.GetNormalizedOffsetGyroX() *
                pointerOffsetMax, gameManager.GyroManager.GetNormalizedOffsetGyroY() * pointerOffsetMax),
                Time.deltaTime * 5.0f);
        }
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    //-------------IReFitGyro----------------
    void GyroInputUp()
    {
        hudUp.SetActive(true);
    }
    void GyroInputDown()
    {
        hudDown.SetActive(true);
    }
    void GyroInputLeft()
    {
        hudLeft.SetActive(true);
    }
    void GyroInputRight()
    {
        hudRight.SetActive(true);
    }
    
    //---------------------------------------------------------------
    //인수로 들어온 enum에 따라 GyroInput 함수를 호출
    public void GyroInputEnter(GyroDirection gyroDirection)
    {
        switch (gyroDirection)
        {
            case GyroDirection.Up:
                GyroInputUp();
                break;
            case GyroDirection.Down:
                GyroInputDown();
                break;
            case GyroDirection.Left:
                GyroInputLeft();
                break;
            case GyroDirection.Right:
                GyroInputRight();
                break;
            case GyroDirection.Center:
                break;
        }
    }
}